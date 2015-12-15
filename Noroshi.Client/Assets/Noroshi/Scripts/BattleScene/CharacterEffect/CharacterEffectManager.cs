using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.CharacterEffect
{
    public class CharacterEffectManager : IManager
    {
        const int FRONT_MAX_ORDER =  300;
        const int FRONT_MIN_ORDER =    6;
        const int BACK_MAX_ORDER  =   -6;
        const int BACK_MIN_ORDER  = -300;
        
        Dictionary<uint, Core.WebApi.Response.CharacterEffect> _dataMap;
        Dictionary<ICharacterView, Dictionary<uint, ICharacterEffectView>> _playingViews = new Dictionary<ICharacterView, Dictionary<uint, ICharacterEffectView>>();
        Dictionary<ICharacterView, int> _maxFrontOrders = new Dictionary<ICharacterView, int>();
        Dictionary<ICharacterView, int> _minBackOrders = new Dictionary<ICharacterView, int>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public void Initialize()
        {
            // BattleManager が通知してくるキャラクターエフェクト命令に処理を紐付ける。
            SceneContainer.GetBattleManager().GetOnCommandCharacterEffectObservable()
            .SelectMany(e => _onCommandCharacterEffect(e))
            .Subscribe().AddTo(_disposables);
        }

        public IObservable<IManager> LoadDatas()
        {
            return GlobalContainer.RepositoryManager.CharacterEffectRepository.LoadAll()
            .Do(datas => _dataMap = datas.ToDictionary(d => d.ID))
            .Select(_ => (IManager)this);
        }

        public IObservable<IManager> LoadAssets(IFactory factory)
        {
            var characterFactory = (ICharacterFactory)factory;
            // 事前に必要なアセットを十分なだけロードしておく。
            var prefabIds = _dataMap.Values.Select(d => d.PrefabID).Distinct();
            return SceneContainer.GetCacheManager().PreloadCharacterEffectView(factory, prefabIds)
            .Select(_ => (IManager)this);
        }
        public void Prepare()
        {
        }

        IObservable<ICharacterEffectView> _onCommandCharacterEffect(CharacterEffectEvent characterEffectEvent)
        {
            // マスター設定ミスの水際チェック
            if (!_dataMap.ContainsKey(characterEffectEvent.CharacterEffectID))
            {
                GlobalContainer.Logger.Error(string.Format("No Master 'CharacterEffect' (ID = {0})", characterEffectEvent.CharacterEffectID));
                return Observable.Empty<ICharacterEffectView>();
            }
            IObservable<ICharacterEffectView> observable;
            // 命令内容によって処理を分岐
            switch (characterEffectEvent.Command)
            {
                case CharacterEffectCommand.Play:
                    observable = _play(characterEffectEvent.CharacterEffectID, characterEffectEvent.CharacterView);
                    break;
                case CharacterEffectCommand.Stop:
                    observable = _stop(characterEffectEvent.CharacterEffectID, characterEffectEvent.CharacterView);
                    break;
                case CharacterEffectCommand.PlayOnce:
                    observable = _playOnce(characterEffectEvent.CharacterEffectID, characterEffectEvent.CharacterView);
                    break;
                case CharacterEffectCommand.Interrupt:
                    observable = _interrupt(characterEffectEvent.CharacterEffectID, characterEffectEvent.CharacterView);
                    break;
                default:
                    observable = Observable.Empty<ICharacterEffectView>();
                    break;
            }
            return observable;
        }

        IObservable<ICharacterEffectView> _play(uint characterEffectId, ICharacterView characterView)
        {
            return _play(characterEffectId, characterView, true);
        }
        IObservable<ICharacterEffectView> _playOnce(uint characterEffectId, ICharacterView characterView)
        {
            return _play(characterEffectId, characterView, false);
        }
        IObservable<ICharacterEffectView> _play(uint characterEffectId, ICharacterView characterView, bool loopPlay)
        {
            var data = _dataMap[characterEffectId];
            // 該当 Prefab を（極力）キャッシュプールからビルドして、
            return _buildCharacterEffectView(data.PrefabID)
            .SelectMany(v => 
            {
                // 既に再生中なのであれば止める。
                var playingView = _getPlayingView(characterView, characterEffectId);
                if (playingView != null)
                {
                    playingView.Stop();
                }
                if (data.HasText)
                {
                    _playText(characterEffectId, characterView).Subscribe().AddTo(_disposables);
                }
                _setPlayingView(characterView, characterEffectId, v);
                var order = _getOrder(characterView, data.OrderInCharacterLayer);
                // 再生。
                // 再生が終わったタイミングで該当ビューはストックしておく。
                var playObservable = !loopPlay
                ? v.PlayOnce(characterView, data.AnimationName, (PositionInCharacterView)data.Position, order)
                : data.IsMultiAnimation ? v.Play(characterView, data.AnimationName + "_appear", data.AnimationName + "_wait", (PositionInCharacterView)data.Position, order)
                : v.Play(characterView, data.AnimationName, (PositionInCharacterView)data.Position, order);
                return playObservable.Do(_ => _stock(characterEffectId, v, characterView))
                .Select(_ => v);
            });
        }
        IObservable<ICharacterEffectView> _playText(uint characterEffectId, ICharacterView characterView)
        {
            var data = _dataMap[characterEffectId];
            // 該当 Prefab を（極力）キャッシュプールからビルドして、
            return _buildCharacterEffectView(data.PrefabID)
                .SelectMany(v => 
                {
                    var order = _getOrder(characterView, 1); // FIXED ORDER
                    // 再生が終わったタイミングで該当ビューはストックしておく。
                    var playObservable = v.PlayOnce(characterView, data.AnimationName + "_text", PositionInCharacterView.Top, order, true);
                    return playObservable.Do(_ => _stock(characterEffectId, v, characterView)).Select(_ => v);
                });
        }
        IObservable<ICharacterEffectView> _interrupt(uint characterEffectId, ICharacterView characterView)
        {
            var data = _dataMap[characterEffectId];
            // 該当 Prefab を（極力）キャッシュプールからビルドして、
            return _buildCharacterEffectView(data.PrefabID)
            .SelectMany(v =>
            {
                // 既に再生中なのであれば止める。
                var playingView = _getPlayingView(characterView, characterEffectId);
                if (playingView != null)
                {
                    playingView.Stop();
                }
                _setPlayingView(characterView, characterEffectId, v);
                var order = _getOrder(characterView, data.OrderInCharacterLayer);
                // 再生。
                return v.Play(characterView, data.AnimationName + "_damage", data.AnimationName + "_wait", (PositionInCharacterView)data.Position, order)
                .Do(_ => _stock(characterEffectId, v, characterView))
                .Select(_ => v);
            });
        }
        IObservable<ICharacterEffectView> _buildCharacterEffectView(uint prefabId)
        {
            return SceneContainer.GetCacheManager().ForceGetCharacterEffectCache(SceneContainer.GetFactory(), prefabId).Get()
            .Do(v => v.SetActive(true));
        }
        
        IObservable<ICharacterEffectView> _stop(uint characterEffectId, ICharacterView characterView)
        {
            var data = _dataMap[characterEffectId];
            var view = _getPlayingView(characterView, characterEffectId);
            if (data.IsMultiAnimation)
            {
                view.StopWithPlayOnce(data.AnimationName + "_disappear");
            }
            else
            {
                view.Stop();
            }
            return Observable.Return<ICharacterEffectView>(view);
        }
        void _stock(uint characterEffectId, ICharacterEffectView characterEffectView, ICharacterView characterView)
        {
            _removePlayingView(characterView, characterEffectId);
            characterEffectView.RemoveParent();
            characterEffectView.SetActive(false);
            SceneContainer.GetCacheManager().GetCharacterEffectCache(_dataMap[characterEffectId].PrefabID).Stock(characterEffectView);
        }

        ICharacterEffectView _getPlayingView(ICharacterView characterView, uint characterEffectId)
        {
            if (_playingViews.ContainsKey(characterView) && _playingViews[characterView].ContainsKey(characterEffectId))
            {
                return _playingViews[characterView][characterEffectId];
            }
            return null;
        }
        void _setPlayingView(ICharacterView characterView, uint characterEffectId, ICharacterEffectView characterEffectView)
        {
            if (!_playingViews.ContainsKey(characterView))
            {
                _playingViews.Add(characterView, new Dictionary<uint, ICharacterEffectView>());
            }
            _playingViews[characterView].Add(characterEffectId, characterEffectView);
        }
        int _getOrder(ICharacterView characterView, int orderInCharacterLayer)
        {
            if (orderInCharacterLayer > 0)
            {
                if (!_maxFrontOrders.ContainsKey(characterView) || _maxFrontOrders[characterView] >= FRONT_MAX_ORDER)
                {
                    _maxFrontOrders[characterView] = FRONT_MIN_ORDER;
                }
                else
                {
                    _maxFrontOrders[characterView]++;
                }
                return _maxFrontOrders[characterView];
            }
            else
            {
                if (!_minBackOrders.ContainsKey(characterView) || _minBackOrders[characterView] <= BACK_MIN_ORDER)
                {
                    _minBackOrders[characterView] = BACK_MAX_ORDER;
                }
                else
                {
                    _minBackOrders[characterView]--;
                }
                return _minBackOrders[characterView];
            }
        }
        void _removePlayingView(ICharacterView characterView, uint characterEffectId)
        {
            _playingViews[characterView].Remove(characterEffectId);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}