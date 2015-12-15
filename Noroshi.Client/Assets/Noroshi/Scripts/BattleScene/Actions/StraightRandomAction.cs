using UniLinq;
using UniRx;
using System.Collections.Generic;

namespace Noroshi.BattleScene.Actions
{
    public class StraightRandomAction : AbstractMultiAction
    {
        int _executeNum = 0;
        protected override uint? _actionId1 { get { return _arg1 > 0 ? (uint?)_arg1 : null; } }
        protected override uint? _actionId2 { get { return _arg2 > 0 ? (uint?)_arg2 : null; } }
        protected override uint? _actionId3 { get { return _arg3 > 0 ? (uint?)_arg3 : null; } }
        protected override uint? _actionId4 { get { return _arg4 > 0 ? (uint?)_arg4 : null; } }
        protected override uint? _actionId5 { get { return _arg5 > 0 ? (uint?)_arg5 : null; } }
        protected override uint? _actionId6 { get { return _arg6 > 0 ? (uint?)_arg6 : null; } }
        protected override uint? _actionId7 { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        protected override uint? _actionId8 { get { return _arg8 > 0 ? (uint?)_arg8 : null; } }

        public StraightRandomAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return actionTargetFinder.GetAllTargets().Where(t => t.Force != executor.CurrentForce).ToArray();
        }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _executeNum = 0;
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _executeNum++;
            if (_executeNum == 1)
            {
                executor.GoStraight(_duration);
            }
            else
            {
                _act(actionTargetFinder, executor);
            }
        }

        public override void PostProcess(IActionExecutor executor)
        {
            base.PostProcess(executor);
            executor.Appear();
        }

        void _act(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var action = GlobalContainer.RandomGenerator.Lot<IAction>(_actions);
            var thisTargets = action.GetTargets(actionTargetFinder, executor);
            if (thisTargets.Length > 0)
            {
                action.Execute(actionTargetFinder, executor, thisTargets);
            }
        }
        float _duration { get { return _animation.FinishTime - _animation.TriggerTimes[0]; } }
    }
}