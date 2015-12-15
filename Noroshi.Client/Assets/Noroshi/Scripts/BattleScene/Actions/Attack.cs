using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class Attack : AbstractNoRangeAttack
    {
        public Attack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected override ActionEffectType? _actionEffectType { get { return _arg4 > 0 ? (ActionEffectType?)_arg4 : null; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            // TODO
            _tryActionViewAppearance(targetCandidates[0].GetActionTargetView().GetPosition(), executor.GetDirection(), AnimationName, 1000);
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            _attackable.AttackMulti(executor, _noRangeSearchable.FilterTargets(actionTargetFinder, targetCandidates));
            _tryCameraShake();
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