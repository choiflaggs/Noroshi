using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class ChargeRangeAttack : AbstractRangeAttack
    {
        ChargeAttackable _chargeAttackable;

        public ChargeRangeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        protected override float? _damageCoefficient { get { return _arg4 > 0 ? (float?)_arg4 / 100.0f : null; } }
        protected int _attackMinRange { get { return _arg5; } }
        protected int _attackMaxRange { get { return _arg6; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            // アニメーションロード後でないとダメなので一旦ここで。
            _chargeAttackable = new ChargeAttackable()
            {
                MaxTargetNum = _maxTargetNum,
                DamageType = DamageType.Value,
                Force = _targetRelativeForce,
                FirstActionMinRange = _attackMinRange,
                FirstActionMaxRange = _attackMaxRange,
                DamageBoost = _getDamageBoost(),
                TargetStateID = _targetStateId,
                AttributeID = _attributeId,
                AttributeCoefficient = _getLevelDrivenAttributeParam(),
                HitCharacterEffectID = _hitCharacterEffectID,
                Animation = _animation,
                DamageCoefficient = _damageCoefficient,
                TargetSortType = _targetSortType,
            };
            _chargeAttackable.Initialize();
            _chargeAttackable.PreProcess();
        }
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _chargeAttackable.ExecuteAndGoBack(actionTargetFinder, executor, targets);
        }
    }
}
