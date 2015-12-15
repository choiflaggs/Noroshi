using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class RatioDamage : AbstractNoRangeAction
    {
        RatioDamageable _ratioDamageable;
        public RatioDamage(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _ratioDamageable = new RatioDamageable(
                _targetStateId,
                _attributeId, _getLevelDrivenAttributeParam(),
                _hitCharacterEffectID
            );
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        int? _hpRatio { get { return _arg3 != 0 ? (int?)_arg3 : null; } }
        int? _energyRatio { get { return _arg4 != 0 ? (int?)_arg4 : null; } }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _ratioDamageable.RatioDamageMulti(executor, _noRangeSearchable.FilterTargets(actionTargetFinder, targets), _getHPDamageRatio(), _getEnergyDamageRatio());
        }

        float? _getHPDamageRatio()
        {
            return _hpRatio.HasValue ? (float?)(_getLevelDrivenParam1() / 100.0f * _hpRatio.Value) : null;
        }
        float? _getEnergyDamageRatio()
        {
            return _energyRatio.HasValue ? (float?)(_getLevelDrivenParam1() / 100.0f * _energyRatio.Value) : null;
        }
    }
}