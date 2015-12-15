using Noroshi.BattleScene.Actions.Roles;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public class ChargeAttack : AbstractNoRangeAttack
    {
        ChargeAttackable _chargeAttackable;

        public ChargeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected int _attackMinRange { get { return _arg4; } }
        protected int _attackMaxRange { get { return _arg5; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg6 > 0 ? (ActionEffectType?)_arg6 : null; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            // アニメーションロード後でないとダメなので一旦ここで。
            _chargeAttackable = new ChargeAttackable()
            {
                MaxTargetNum = _maxTargetNum,
                DamageType = DamageType.Value,
                Force = RelativeForce.Enemy,
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
            _chargeAttackable.ExecuteAndGoBack(actionTargetFinder, executor, targets, () => 
            {
                _tryCameraShake();
            });
        }
    }
}
