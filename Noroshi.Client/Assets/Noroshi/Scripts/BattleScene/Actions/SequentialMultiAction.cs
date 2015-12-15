using System.Collections.Generic;
using UniLinq;

namespace Noroshi.BattleScene.Actions
{
    public class SequentialMultiAction : AbstractMultiAction
    {
        int _currentActionIndex;
        
        protected override uint? _actionId1 { get { return _arg1 > 0 ? (uint?)_arg1 : null; } }
        protected override uint? _actionId2 { get { return _arg2 > 0 ? (uint?)_arg2 : null; } }
        protected override uint? _actionId3 { get { return _arg3 > 0 ? (uint?)_arg3 : null; } }
        protected override uint? _actionId4 { get { return _arg4 > 0 ? (uint?)_arg4 : null; } }
        protected override uint? _actionId5 { get { return _arg5 > 0 ? (uint?)_arg5 : null; } }
        protected override uint? _actionId6 { get { return _arg6 > 0 ? (uint?)_arg6 : null; } }
        protected override uint? _actionId7 { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        protected override uint? _actionId8 { get { return _arg8 > 0 ? (uint?)_arg8 : null; } }

        public SequentialMultiAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _actions[0].GetTargets(actionTargetFinder, executor);
        }
        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _currentActionIndex = 0;
        }
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            if (_currentActionIndex >= _actions.Length - 1) _currentActionIndex = _actions.Length - 1;
            _actions[_currentActionIndex].Execute(actionTargetFinder, executor, _actions[_currentActionIndex].GetTargets(actionTargetFinder, executor));
            _currentActionIndex++;
        }
    }
}