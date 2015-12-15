using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene
{
    public class CharacterSet
    {
        const byte MIN_CHARACTER_NO = 1;
        const bool DEFAULT_AUTO = false;

        byte _currentCharacterNo = MIN_CHARACTER_NO - 1;
        bool _auto = DEFAULT_AUTO;
        readonly List<Character> _characters = new List<Character>();
        readonly Dictionary<byte, uint> _damages = new Dictionary<byte, uint>();
        readonly Subject<ShadowCharacter> _onAddShadowSubject = new Subject<ShadowCharacter>();
        readonly Subject<ShadowCharacter> _onRemoveShadowSubject = new Subject<ShadowCharacter>();
        readonly CompositeDisposable _disposables = new CompositeDisposable();

        public readonly Force Force;

        public CharacterSet(Force force, bool auto = DEFAULT_AUTO)
        {
            Force = force;
            _auto = auto;
        }

        public IObservable<ShadowCharacter> GetOnAddShadowObservable()
        {
            return _onAddShadowSubject.AsObservable();
        }
        public IObservable<ShadowCharacter> GetOnRemoveShadowObservable()
        {
            return _onRemoveShadowSubject.AsObservable();
        }

        public IObservable<CharacterSet> LoadDatas()
        {
            return Observable.WhenAll(_characters.Select(c => c.LoadDatas())).Select(_ => this);
        }
        public IObservable<CharacterSet> LoadAssets(IFactory factory, IActionFactory actionFactory)
        {
            return Observable.WhenAll(_characters.Select(c => c.LoadAssets(actionFactory))).Select(_ => this);
        }

        public Character[] GetCharacters()
        {
            // そのまま外には出さない
            return _characters.ToArray();
        }
        public Character GetCharacterByNo(byte no)
        {
            return _characters.Where(c => c.No == no).FirstOrDefault();
        }
        public Character GetPickUpCharacter()
        {
            return _damages.OrderByDescending(kv => kv.Value).Select(kv => GetCharacterByNo(kv.Key)).Where(c => !c.IsDead).FirstOrDefault();
        }
        /// サムネイル表示に利用するオブジェクトを返す。
        public IEnumerable<CharacterThumbnail> GetCharacterThumbnails()
        {
            return _characters.Select(c => c.BuildCharacterThumbnail());
        }
        /// 内包するキャラクターいずれかが生きていれば真を返す。
        public bool AreAlive()
        {
            return _characters.Any(pc => !pc.IsDead);
        }

        public void SetCharacters(IEnumerable<Character> characters)
        {
            if (_characters.Count() > 0)
            {
                throw new Exception();
            }
            foreach (var character in characters)
            {
                AddCharacter(character);
            }
        }
        public void AddCharacter(Character character)
        {
            character.SetNo(++_currentCharacterNo);
            character.SetForce(Force);
            if (_auto) character.SetAuto(_auto);
            _characters.Add(character);

            _damages.Add(_currentCharacterNo, 0);

            _subscribeCharacterObservable(character);
        }
        void _subscribeCharacterObservable(Character character)
        {
            character.GetOnAddDamageObservable().Do(damage => _addDamage(character.No, damage))
            .Subscribe().AddTo(_disposables);

            character.ShadowHandler.GetOnMakeShadowObservable().Do(shadow => _onAddShadowSubject.OnNext(shadow))
            .Subscribe().AddTo(_disposables);

            character.ShadowHandler.GetOnRemoveShadowObservable().Do(shadow => _onRemoveShadowSubject.OnNext(shadow))
            .Subscribe().AddTo(_disposables);
        }

        public uint GetTotalCurrentHP()
        {
            return (uint)_characters.Sum(c => c.CurrentHP);
        }
        public uint GetTotalMaxHP()
        {
            return (uint)_characters.Sum(c => c.MaxHP);
        }

        public IObservable<ChangeableValueEvent> GetOnTotalHPChangeObservable()
        {
            return _characters.Select(c => c.GetOnHPChangeObservable()).Merge()
                .Select(hpEvent => new ChangeableValueEvent(hpEvent.Difference, (int)_characters.Sum(c => c.CurrentHP), (int)_characters.Sum(c => c.MaxHP)));
        }

        public void RemoveCharacter(Character character)
        {
            _characters.Remove(character);
        }

        public IEnumerable<uint> GetDamages()
        {
            return Enumerable.Range(MIN_CHARACTER_NO, _currentCharacterNo).Select(no => _damages[(byte)no]);
        }
        void _addDamage(byte characterNo, int damage)
        {
            if (damage <= 0) return;
            _damages[characterNo] += (uint)damage;
        }

        public void SetActive(bool active)
        {
            foreach (var character in _characters)
            {
                character.GetView().SetActive(active);
            }
        }

        public void SetAuto(bool auto)
        {
            _auto = auto;
            foreach (var character in _characters)
            {
                character.SetAuto(auto);
            }
        }

        public void RecoveryAtIntervalEnter()
        {
            foreach (var character in _characters.Where(c => !c.IsDead))
            {
                character.RecoverAtInterval();
            }
        }

        public void PrepareNextWave()
        {
            foreach (var character in _characters)
            {
                character.PrepareNextWave();
                _damages[character.No] = 0;
            }
        }
        public void Clear()
        {
            _disposables.Clear();

            _characters.Clear();
            _currentCharacterNo = MIN_CHARACTER_NO - 1;

            _damages.Clear();
        }
        public void Dispose()
        {
            _disposables.Dispose();
            foreach (var character in _characters)
            {
                character.Dispose();
            }
        }
    }
}