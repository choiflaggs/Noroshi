using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene
{
    /// バトル内で利用するためのキャラクターステータスクラス。パフォーマンスを上げるために計算結果はキャッシュする作り。
    public class CharacterStatus : Game.CharacterStatus
    {
        readonly StatusBooster _statusBooster = new StatusBooster();
        int _strength;
        int _intellect;
        int _agility;
        uint _maxHp;
        uint _physicalAttack;
        uint _magicPower;
        uint _armor;
        uint _magicRegistance;
        uint _physicalCrit;
        byte _accuracy;
        byte _dodge;
        uint _lifeStealRating;
        float _actionFrequency;
        Subject<CharacterStatusBoostEvent> _onChangeStatusBooster = new Subject<CharacterStatusBoostEvent>();

        readonly StatusForceSetter _statusForceSetter = new StatusForceSetter();

        public CharacterStatus(IPersonalCharacter personalData, Core.WebApi.Response.Character.Character masterData) : base(personalData, masterData)
        {
            _calculate();
        }

        public IObservable<CharacterStatusBoostEvent> GetOnChangeStatusBooster()
        {
            return _onChangeStatusBooster.AsObservable();
        }

        public new IObservable<CharacterStatus> LoadGears()
        {
            return base.LoadGears().Cast<Game.CharacterStatus, CharacterStatus>();
        }

        public void OverrideLevels(ushort? overrideLevel, ushort? overrideActionLevel2, ushort? overrideActionLevel3)
        {
            if (overrideLevel.HasValue) _overrideLevel(overrideLevel.Value);
            if (overrideActionLevel2.HasValue) _overrideActionLevel2(overrideActionLevel2.Value);
            if (overrideActionLevel3.HasValue) _overrideActionLevel2(overrideActionLevel3.Value);
            _calculate();
        }

        public void AddStatusBreakerFactor(StatusForceSetter.Factor factor)
        {
            _statusForceSetter.AddFactor(factor);
            _calculate();
        }
        public void RemoveStatusBreakerFactor(StatusForceSetter.Factor factor)
        {
            _statusForceSetter.RemoveFactor(factor);
            _calculate();
        }

        public void AddStatusBoosterFactor(IStatusBoostFactor factor)
        {
            _statusBooster.AddFactor(factor);
            _calculate();
            _onChangeStatusBooster.OnNext(new CharacterStatusBoostEvent()
            {
                Type = CharacterStatusBoostEvent.EventType.Add,
                StatusBooster = _statusBooster,
                StatusBoosterFactor = factor,
            });
        }
        public void AddStatusBoosterFactors(IEnumerable<IStatusBoostFactor> factors)
        {
            _statusBooster.AddFactors(factors);
            _calculate();
            foreach (var factor in factors)
            {
                _onChangeStatusBooster.OnNext(new CharacterStatusBoostEvent()
                {
                    Type = CharacterStatusBoostEvent.EventType.Add,
                    StatusBooster = _statusBooster,
                    StatusBoosterFactor = factor,
                });
            }
        }
        public void RemoveStatusBoosterFactor(IStatusBoostFactor factor){
            _statusBooster.RemoveFactor(factor);
            _calculate();
            _onChangeStatusBooster.OnNext(new CharacterStatusBoostEvent()
            {
                Type = CharacterStatusBoostEvent.EventType.Remove,
                StatusBooster = _statusBooster,
                StatusBoosterFactor = factor,
            });
        }

        uint _calculateArmor()
        {
            return (uint)(_statusForceSetter.Armor.HasValue ? _statusForceSetter.Armor.Value : base.Armor + _statusBooster.Armor);
        }

        uint _caluclateMagicRegistance()
        {
            return (uint)(_statusForceSetter.MagicRegistance.HasValue ? _statusForceSetter.MagicRegistance.Value : base.MagicRegistance + _statusBooster.MagicRegistance);
        }

        void _calculate()
        {
            _strength        = (int)(base.Strength        + _statusBooster.Strength);
            _intellect       = (int)(base.Intellect       + _statusBooster.Intellect);
            _agility         = (int)(base.Agility         + _statusBooster.Agility);
            _physicalAttack  = (uint)(base.PhysicalAttack  + _statusBooster.PhysicalAttack);
            _magicPower      = (uint)(base.MagicPower      + _statusBooster.MagicPower);
            _armor           = _calculateArmor();
            _magicRegistance = _caluclateMagicRegistance();
            _physicalCrit    = base.PhysicalCrit;
            _accuracy        = (byte)(base.Accuracy        + _statusBooster.Accuracy);
            _dodge           = (byte)(base.Dodge           + _statusBooster.Dodge);
            _lifeStealRating = (uint)(base.LifeStealRating + _statusBooster.LifeStealRating);
            _actionFrequency = base.ActionFrequency + _statusBooster.ActionFrequency;
            _maxHp           = (uint)(base.MaxHP + _statusBooster.MaxHp);
        }

        public override float Strength  { get { return _strength; } }
        public override float Intellect { get { return _intellect; } }
        public override float Agility   { get { return _agility; } }
        /// 最大 HP。
        public override uint MaxHP { get { return _maxHp; } }
        /// 物理攻撃力。
        public override uint PhysicalAttack { get { return _physicalAttack; } }
        /// 魔法攻撃力。
        public override uint MagicPower { get { return _magicPower; } }
        /// 物理防御力。
        public override uint Armor { get { return _armor; } }
        /// 魔法防御力。
        public override uint MagicRegistance { get { return _magicRegistance; } }
        /// 物理クリティカル。
        public override uint PhysicalCrit { get { return _physicalCrit; } }
        /// 命中。
        public override byte Accuracy { get { return _accuracy; } }
        /// 回避。
        public override byte Dodge { get { return _dodge; } }
        /// ライフ奪取。
        public override uint LifeStealRating { get { return _lifeStealRating; } }
        public override float ActionFrequency { get { return _actionFrequency; } }

        public BattleScene.Actions.ActionSequence ActionSequence
        {
            get
            {
                return new BattleScene.Actions.ActionSequence(_masterData.ActionSequences.Where(cas => cas.TargetActionNum == AvailableActionIDs.Count()).First());
            }
        }
    }
}
