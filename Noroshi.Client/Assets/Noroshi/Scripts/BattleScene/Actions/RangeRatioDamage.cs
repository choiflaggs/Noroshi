using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class RangeRatioDamage : AbstractRangeAction
    {
        RatioDamageable _ratioDamageable;
        public RangeRatioDamage(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _ratioDamageable = new RatioDamageable(
                _targetStateId,
                _attributeId, _getLevelDrivenAttributeParam(),
                _hitCharacterEffectID
            );
        }
        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        int? _hpRatio { get { return _arg4 != 0 ? (int?)_arg4 : null; } }
        int? _energyRatio { get { return _arg5 != 0 ? (int?)_arg5 : null; } }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _ratioDamageable.RatioDamageMulti(executor, targets, _getHPDamageRatio(), _getEnergyDamageRatio());
        }

        float? _getHPDamageRatio()
        {
            return _hpRatio.HasValue ? (float?)(_getLevelDrivenParam1() / 100 * _hpRatio.Value) : null;
        }
        float? _getEnergyDamageRatio()
        {
            return _energyRatio.HasValue ? (float?)(_getLevelDrivenParam1() / 100 * _energyRatio.Value) : null;
        }
    }
}
