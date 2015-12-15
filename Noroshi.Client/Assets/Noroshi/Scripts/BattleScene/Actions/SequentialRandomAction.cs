using UniLinq;
using System.Collections.Generic;

namespace Noroshi.BattleScene.Actions
{
    public class SequentialRandomAction : AbstractMultiAction
    {
        protected override uint? _actionId1 { get { return _arg1 > 0 ? (uint?)_arg1 : null; } }
        protected override uint? _actionId2 { get { return _arg2 > 0 ? (uint?)_arg2 : null; } }
        protected override uint? _actionId3 { get { return _arg3 > 0 ? (uint?)_arg3 : null; } }
        protected override uint? _actionId4 { get { return _arg4 > 0 ? (uint?)_arg4 : null; } }
        protected override uint? _actionId5 { get { return _arg5 > 0 ? (uint?)_arg5 : null; } }
        protected override uint? _actionId6 { get { return _arg6 > 0 ? (uint?)_arg6 : null; } }
        protected override uint? _actionId7 { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        protected override uint? _actionId8 { get { return _arg8 > 0 ? (uint?)_arg8 : null; } }

        public SequentialRandomAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _actions[0].GetTargets(actionTargetFinder, executor);
        }
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
             var currentActionIndex = GlobalContainer.RandomGenerator.GenerateInt(_actions.Count());
            _actions[currentActionIndex].Execute(actionTargetFinder, executor, targetCandidates);
        }
    }
}