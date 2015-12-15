using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Character;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene
{
    /// フィールドに配置されていない状態も含めてキャラクターを管理するクラス。
    /// 逆にフィールドに配置されているキャラクター操作は極力 BattleManager / Wave / Field へ寄せる。
    public class CharacterManager : IManager
    {
        int _currentEnemyCharacterSetIndex = 0;
        List<CharacterSet> _enemyCharacterSets = new List<CharacterSet>();
        readonly CharacterSet _ownShadowSet   = new CharacterSet(Force.Own);
        readonly CharacterSet _enemyShadowSet = new CharacterSet(Force.Enemy, true);
        Subject<ShadowCharacter> _onCompleteAddingShadowSubject = new Subject<ShadowCharacter>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public CharacterSet CurrentOwnCharacterSet { get; private set; }
        public CharacterSet CurrentEnemyCharacterSet { get { return _enemyCharacterSets[_currentEnemyCharacterSetIndex]; } }

        public void Initialize()
        {
            CurrentOwnCharacterSet = new CharacterSet(Force.Own);
        }
        void _subscribeCharacterSetObservables(CharacterSet characterSet)
        {
            characterSet.GetOnAddShadowObservable().Do(s => _makeShadow(s))
            .Subscribe().AddTo(_disposables);
            characterSet.GetOnRemoveShadowObservable().Do(s => _removeShadow(s))
            .Subscribe().AddTo(_disposables);
        }
        public IObservable<IManager> LoadDatas()
        {
            var observables = _enemyCharacterSets.Select(cs => cs.LoadDatas()).ToList();
            observables.Add(CurrentOwnCharacterSet.LoadDatas());
            return Observable.WhenAll(observables)
                .Select(_ => (IManager)this);
        }
        public IObservable<IManager> LoadAssets(IFactory factory)
        {
            var actionFactory = (IActionFactory)factory;
            var observables = _enemyCharacterSets.Select(cs => cs.LoadAssets(factory, actionFactory)).ToList();
            observables.Add(CurrentOwnCharacterSet.LoadAssets(factory, actionFactory));
            return Observable.WhenAll(observables)
            .Do(_ => _activateCurrentOwnCharacterSet())
            .Do(_ => _activateCurrentEnemyCharacterSet())
            .Select(_ => (IManager)this);
        }
        public void Prepare()
        {
        }

        void _activateCurrentOwnCharacterSet()
        {
            CurrentOwnCharacterSet.SetActive(true);
        }
        void _activateCurrentEnemyCharacterSet()
        {
            for (var i = 0; i < _enemyCharacterSets.Count(); i++)
            {
                _enemyCharacterSets[i].SetActive(i == _currentEnemyCharacterSetIndex);
            }
        }

        public IObservable<ShadowCharacter> GetOnCompleteAddingShadowObservable()
        {
            return _onCompleteAddingShadowSubject.AsObservable();
        }
        public IObservable<ShadowCharacter> GetOnRemoveShadowObservable()
        {
            return Observable.Merge(
                CurrentOwnCharacterSet.GetOnRemoveShadowObservable(),
                CurrentEnemyCharacterSet.GetOnRemoveShadowObservable()
            );
        }

        /// 自キャラセットメソッド。
        public Character[] SetCurrentOwnPlayerCharacters(IEnumerable<Core.WebApi.Response.Battle.BattleCharacter> battleCharacters)
        {
            var characters = battleCharacters.Select(bc => new PlayerCharacter(bc)).Cast<Character>();
            CurrentOwnCharacterSet.SetCharacters(characters);
            _subscribeCharacterSetObservables(CurrentOwnCharacterSet);
            return CurrentOwnCharacterSet.GetCharacters();
        }
        /// 敵キャラセットメソッド。

        public void SetCurrentEnemyCpuCharacterSets(IEnumerable<IEnumerable<Core.WebApi.Response.Battle.BattleCharacter>> cpuCharacterSets)
        {
            foreach (var cpuCharacterSet in cpuCharacterSets)
            {
                var characterSet = new CharacterSet(Force.Enemy, true);
                characterSet.SetCharacters(cpuCharacterSet.Select(cc => new CpuCharacter(cc)).Cast<Character>());
                _subscribeCharacterSetObservables(characterSet);
                _enemyCharacterSets.Add(characterSet);
            }
        }
        public void SetCurrentEnemyPlayerCharacterSets(IEnumerable<IEnumerable<Core.WebApi.Response.Battle.BattleCharacter>> playerCharacterSets)
        {
            foreach (var playerCharacters in playerCharacterSets)
            {
                var characterSet = new CharacterSet(Force.Enemy, true);
                characterSet.SetCharacters(playerCharacters.Select(pc => new PlayerCharacter(pc)).Cast<Character>());
                _subscribeCharacterSetObservables(characterSet);
                _enemyCharacterSets.Add(characterSet);
            }
        }
        public void SwitchNextEnemyCharacterSet()
        {
            _currentEnemyCharacterSetIndex++;
            // ループバトル対応。
            if (_currentEnemyCharacterSetIndex >= _enemyCharacterSets.Count())
            {
                _currentEnemyCharacterSetIndex = 0;
            }
            // 過去に利用した EnemyCharacterSet の中身は死亡状態のはずなので復活させる。
            foreach (var character in CurrentEnemyCharacterSet.GetCharacters())
            {
                character.Resurrect();
            }
            _activateCurrentEnemyCharacterSet();
        }

        void _makeShadow(ShadowCharacter shadow)
        {
            var shadowSet = shadow.Force == Force.Own ? _ownShadowSet : _enemyShadowSet;
            if (shadowSet.GetCharacters().Count() >= Constant.MAX_SHADOW_CHARACTER_NUM_IN_FIELD_PER_FORCE)
            {
                return;
            }
            shadowSet.AddCharacter(shadow);
            SceneContainer.GetBattleManager().CurrentWave.SetShadowCharacter(shadow);
            shadow.SetViewToCorrectPosition();
            _onCompleteAddingShadowSubject.OnNext(shadow);
            shadow.Start();
            shadow.TryToInvokeFirstAction();
        }
        void _removeShadow(ShadowCharacter shadow)
        {
            var shadowSet = shadow.Force == Force.Own ? _ownShadowSet : _enemyShadowSet;
            shadowSet.RemoveCharacter(shadow);
        }

        /// 現在の自・敵キャラを全て取得
        public Character[] GetCurrentAllCharacters()
        {
            var characters = new List<Character>(GetCurrentOwnCharacters().Cast<Character>());
            characters.AddRange(GetCurrentEnemyCharacters().Cast<Character>());
            return characters.ToArray();
        }
        /// 現在の自キャラを全て取得
        public Character[] GetCurrentOwnCharacters() { return CurrentOwnCharacterSet.GetCharacters(); }
        /// 現在の敵キャラを全て取得
        public Character[] GetCurrentEnemyCharacters() { return CurrentEnemyCharacterSet.GetCharacters(); }

        public IObservable<ChangeableValueEvent> GetOnEnemyCharacterSetHPChangeObservable()
        {
            return CurrentEnemyCharacterSet.GetOnTotalHPChangeObservable();
        }
        public uint GetEnemyCharacterSetTotalCurrentHP()
        {
            return CurrentEnemyCharacterSet.GetTotalCurrentHP();
        }
        public uint GetEnemyCharacterSetTotalMaxHP()
        {
            return CurrentEnemyCharacterSet.GetTotalMaxHP();
        }

        public void RemoveAttributeAndShadow()
        {
            foreach (var character in GetCurrentAllCharacters())
            {
                character.RemoveAttributeAndShadow();
            }
            _ownShadowSet.Clear();
            _enemyShadowSet.Clear();
        }

        public void Dispose()
        {
            CurrentOwnCharacterSet.Dispose();
            CurrentEnemyCharacterSet.Dispose();
            _ownShadowSet.Dispose();
            _enemyShadowSet.Dispose();
            _disposables.Dispose();
        }
    }
}