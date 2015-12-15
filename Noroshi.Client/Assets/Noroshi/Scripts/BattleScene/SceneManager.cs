using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.BattleScene.Cache;
using Noroshi.BattleScene.UI;
using Noroshi.BattleScene.Camera;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene
{
    /// バトルシーン内のロジックを管理するルートクラス。全てのバトルロジックはこのクラス以下で行われる。
    public class SceneManager
    {
        int _frameCount = 1;
        Subject<bool> _onFinishWaiting = new Subject<bool>();
        readonly BattleType _battleType;
        readonly int _logicUpdateInterval = Constant.ENGINE_FPS / Constant.LOGIC_FPS;
        readonly SceneStateHandler _sceneStateHandler = new SceneStateHandler();
        readonly IManager[] _managers;
        readonly IUIManager _uiManager;
        readonly CharacterEffectManager _characterEffectManager = new CharacterEffectManager();
        readonly ICameraManager _cameraManager;
        readonly SoundManager _soundManager = new SoundManager();

        public readonly IBattleManager BattleManager;
        public readonly CharacterManager CharacterManager = new CharacterManager();
        public readonly CacheManager CacheManager = new CacheManager();
        CompositeDisposable _disposables = new CompositeDisposable();

        public SceneManager(BattleCategory battleCategory, uint id, uint[] ownPlayerCharacterIds, uint paymentNum)
        {
            if (battleCategory == BattleCategory.Arena || battleCategory == BattleCategory.Expedition)
            {
                _battleType = BattleType.PlayerBattle;
            }
            else
            {
                _battleType = BattleType.CpuBattle;
            }
            switch (_battleType)
            {
                case BattleType.CpuBattle:
                    BattleManager = new CpuBattleManager(battleCategory, id, ownPlayerCharacterIds, paymentNum);
                    _uiManager = new CpuBattleUIManager();
                    _cameraManager = new CpuBattleCameraManager();
                    break;
                case BattleType.PlayerBattle:
                    BattleManager = new PlayerBattleManager(battleCategory, id, ownPlayerCharacterIds, paymentNum);
                    _uiManager = new PlayerBattleUIManager();
                    _cameraManager = new PlayerBattleCameraManager();
                    break;
                default:
                    break;
            }
            // Manager は配列にも格納し、全 Manager に対する処理はこれを利用する。
            _managers = new IManager[]
            {
                BattleManager,
                CharacterManager,
                _characterEffectManager,
                _uiManager,
                _cameraManager,
                _soundManager,
                CacheManager,
            };
        }

        ///（ブロッキング）初期化処理。各 Manager 毎の相互参照を含んだ初期化が中心。
        public void Initialize()
        {
            for (var i = 0; i < _managers.Length; i++)
            {
                _managers[i].Initialize();
            }
            // PlayerBattle 処理
            if (_battleType == BattleType.PlayerBattle)
            {
                _sceneStateHandler.GetOnEnterReadyObservable()
                .SelectMany(SceneContainer.GetTimeHandler().Timer(Constant.BATTLE_READY_TIME))
                .Subscribe(_ => _sceneStateHandler.TransitToBattle()).AddTo(_disposables);
            }
            else
            {
                _sceneStateHandler.GetOnEnterReadyObservable()
                .SelectMany(_ => _waitToFinishReady())
                .Subscribe(_ => _sceneStateHandler.TransitToBattle()).AddTo(_disposables);
            }
        }

        /// バトルロジックプロセスを開始する。引数はバトルに持ち込む PlayerCharacter の ID を指定。
        public void Start()
        {
            _sceneStateHandler.Start<SceneStateHandler.Initialization>();
        }

        /// 開始時に必要なデータをロードする。
        public IObservable<SceneManager> LoadDatas()
        {
            return _managers.Aggregate(Observable.Return<IManager>(null), (now, next) => now.SelectMany(_ => next.LoadDatas()))
            .Select(_ => this);
        }
        /// 開始時に必要なアセットをロードする。
        public IObservable<SceneManager> LoadAssets()
        {
            var factory = SceneContainer.GetFactory();
            return _managers.Aggregate(Observable.Return<IManager>(null), (now, next) => now.SelectMany(_ => next.LoadAssets(factory)))
            .Select(_ => this);
        }
        ///（ブロッキング）初期化最終処理。アセットロード後でないと実施できない 各 Manager 毎の相互参照を含んだ処理の紐付けなどを行う。
        public void Prepare()
        {
            for (var i = 0; i < _managers.Length; i++)
            {
                _managers[i].Prepare();
            }
        }

        IObservable<bool> _waitToFinishReady()
        {
            // 状態遷移中の処理なので割り込みを避けるべく、何もなくても時間差にする。
            return BattleManager.ShouldWaitToFinishReady() ? _onFinishWaiting.AsObservable() : SceneContainer.GetTimeHandler().Timer(0.01f).Select(_ => false);
        }
        public void FinishReady()
        {
            _onFinishWaiting.OnNext(true);
            _onFinishWaiting.OnCompleted();
        }

        /// シーン遷移時にプッシュされる Observable を取得。
        public IObservable<bool> GetOnTransitSceneObservable()
        {
            return _uiManager.GetOnTransitSceneObservable();
        }
        /// ポーズ切り替え時に On/Off がプッシュされる Observable を取得。
        public IObservable<bool> GetOnTogglePauseObservable()
        {
            return _uiManager.GetOnTogglePauseObservable();
        }
        /// 時間をゆっくり進める時に duration がプッシュされる Observable を取得。
        public IObservable<float> GetOnSlowObservable()
        {
            return BattleManager.GetOnSlowObservable();
        }
        /// 「初期化」状態が終了したタイミングでプッシュされる Observable を取得。
        public IObservable<Type> GetOnExitInitializeObservable()
        {
            return _sceneStateHandler.GetOnExitInitializeObservable();
        }
        /// 「準備」状態が開始したタイミングでプッシュされる Observable を取得。
        public IObservable<BattleType> GetOnEnterReadyObservable()
        {
            return _sceneStateHandler.GetOnEnterReadyObservable().Select(_ => _battleType);
        }
        /// 「結果」状態が開始したタイミングでプッシュされる Observable を取得。
        public IObservable<VictoryOrDefeat> GetOnEnterResultObservable()
        {
            return _sceneStateHandler.GetOnEnterResultObservable();
        }
        /// バトル前ストーリ終了時にプッシュされる Observable を取得。
        public IObservable<bool> GetOnExitBeforeBattleStoryObservable()
        {
            return ((CpuBattleUIManager)_uiManager).GetOnExitBeforeBattleStoryObservable();
        }
        public IObservable<SoundEvent> GetOnCommandSoundObservable()
        {
            return (new[]
            {
                BattleManager.GetOnCommandSoundObservable(),
                _uiManager.GetOnCommandSoundObservable(),
            })
            .Merge();
        }

        /// フレーム毎に呼び出されるロジック更新処理。
        public void UpdatePerFrame()
        {
            // 実際の更新頻度は FPS と一緒とは限らない。
            if (_frameCount % _logicUpdateInterval == 0)
            {
                BattleManager.LogicUpdate();
            }
            _frameCount++;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _sceneStateHandler.Dispose();
            for (var i = 0; i < _managers.Length; i++)
            {
                _managers[i].Dispose();
            }
        }
    }
}
