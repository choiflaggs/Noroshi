using Noroshi.BattleScene.Actions.Roles;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public class RangeAttack : AbstractRangeAttack
    {
        public RangeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        protected override float? _damageCoefficient { get { return _arg4 > 0 ? (float?)_arg4 / 100.0f : null; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg5 > 0 ? (ActionEffectType?)_arg5 : null; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _tryToSetActionViewSkinLevel(executor.SkinLevel);
            _tryActionViewAppearance(targetCandidates[0].GetGridPosition().Value, executor.GetDirection(), AnimationName, 1000);
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            _attackable.AttackMulti(executor, GetTargets(actionTargetFinder, executor));
        }
        
        public override void PostProcess(IActionExecutor executor)
        {
            _tryActionViewDisappearance();
        }
        
        public override void Cancel(IActionExecutor executor)
        {
            _tryActionViewDisappearance();
        }
    }
}