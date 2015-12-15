namespace Noroshi.BattleScene.Actions.Attributes
{
    public class UnlimitedShield : AbstractShield
    {
        public UnlimitedShield(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }
        
        public override bool IsNegative { get { return false; } }
        protected override DamageType? _damageType { get { return _arg1 > 0 ? (DamageType?)_arg1 : null; } }
        protected override float _damageRatio { get { return _arg2 / 100; } }
        protected override float _hpRecoveryRatio { get { return _arg3 / 100; } }
        protected override float _energyRecoveryRatio { get { return _arg4 / 100; } }
    }
}
