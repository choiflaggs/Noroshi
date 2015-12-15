using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class StatusBoost : AbstractAttribute
    {
        StatusBoostFactor _factor;

        public StatusBoost(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
            _factor = new StatusBoostFactor() {
                Strength        = (int)(_strength        * coefficient),
                Intellect       = (int)(_intellect       * coefficient),
                Agility         = (int)(_agility         * coefficient),
                PhysicalAttack  = (int)(_physicalAttack  * coefficient),
                MagicPower      = (int)(_magicPower      * coefficient),
                Armor           = (int)(_armor           * coefficient),
                MagicRegistance = (int)(_magicRegistance * coefficient),
                Accuracy        = (sbyte)(_accuracy        * coefficient),
                Dodge           = (sbyte)(_dodge           * coefficient),
                LifeStealRating = (int)(_lifeStealRating * coefficient),
                ActionFrequency = _actionFrequency * coefficient,
                MaxHp           = (int)(_maxHp * coefficient),
            };
        }

        public override bool CharacterEffectOncePlay { get { return true; } }
        public override bool IsNegative { get { return false; } }

        public override void OnEnter(IActionTarget target)
        {
            target.AddStatusBoosterFactor(_factor);
        }

        public override void OnExit(IActionTarget target)
        {
            target.RemoveStatusBoosterFactor(_factor);
        }

        int   _physicalAttack  { get { return _arg1; } }
        int   _magicPower      { get { return _arg2; } }
        int   _armor           { get { return _arg3; } }
        int   _magicRegistance { get { return _arg4; } }
        sbyte _dodge           { get { return (sbyte)_arg5; } }
        sbyte _accuracy        { get { return (sbyte)_arg6; } }
        int   _agility         { get { return _arg7; } }
        int   _lifeStealRating { get { return _arg8; } }
        float _actionFrequency { get { return (float)_arg9 / 100; } }
        int   _maxHp           { get { return _arg10; } }
        int   _strength        { get { return _arg11; } }
        int   _intellect       { get { return _arg12; } }
    }
}
