using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class FixedDamage : AbstractNoRangeAction
    {
        Damageable _damageable;
        public FixedDamage(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _damageable = new Damageable(
                DamageType, _damageMagicalAttribute,
                _targetStateId,
                _attributeId, _getLevelDrivenAttributeParam(),
                _hitCharacterEffectID
            );
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _damageable.DamageMulti(executor, _noRangeSearchable.FilterTargets(actionTargetFinder, targets), _getDamage(), null);
        }

        int _getDamage()
        {
            return (int)_getLevelDrivenParam1();
        }
    }
}
