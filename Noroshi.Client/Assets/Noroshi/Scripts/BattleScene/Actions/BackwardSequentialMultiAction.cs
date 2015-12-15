using Noroshi.BattleScene.Actions.Roles;
using UniLinq;
using System;

namespace Noroshi.BattleScene.Actions
{
    public class BackwardSequentialMultiAction : AbstractMultiAction
    {
        int _executeNum;
        int _currentActionIndex;
        RangeSearchable _rangeSearchable;

        int _minRange { get { return _arg1; } }
        int _maxRange { get { return _arg2; } }
        int _backStepHorizontalIndex { get { return _arg3; } }
        protected override uint? _actionId1 { get { return _arg4 > 0 ? (uint?)_arg4 : null; } }
        protected override uint? _actionId2 { get { return _arg5 > 0 ? (uint?)_arg5 : null; } }
        protected override uint? _actionId3 { get { return _arg6 > 0 ? (uint?)_arg6 : null; } }
        protected override uint? _actionId4 { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        protected override uint? _actionId5 { get { return _arg8 > 0 ? (uint?)_arg8 : null; } }
        protected override uint? _actionId6 { get { return _arg9 > 0 ? (uint?)_arg9 : null; } }
        protected override uint? _actionId7 { get { return _arg10 > 0 ? (uint?)_arg10 : null; } }
        protected override uint? _actionId8 { get { return null; } }

        public BackwardSequentialMultiAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _rangeSearchable = new RangeSearchable()
            {
                Force = RelativeForce.Enemy,
                MinRange = _minRange,
                MaxRange = _maxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = _targetSortType,
            };
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _rangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce).ToArray();
        }

        public override void PreProcess(Noroshi.BattleScene.Actions.IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess (executor, targetCandidates);
            _currentActionIndex = 0;
            _executeNum = 0;
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _executeNum++;
            if (_executeNum == 1)
            {
                executor.HorizontalMove((short)-_backStepHorizontalIndex, _forwardDuration);
            }
            else if (_executeNum == 2)
            {
                // 止まるだけ
            }
            else
            {
                //  登録されている分だけアクションを実行
                if (_currentActionIndex >= _actions.Length - 1) _currentActionIndex = _actions.Length - 1;
                var attackTargets = _actions[_currentActionIndex].GetTargets(actionTargetFinder, executor);
                _actions[_currentActionIndex++].Execute(actionTargetFinder, executor, attackTargets);
            }
        }

        float _forwardDuration { get { return _animation.TriggerTimes[1] - _animation.TriggerTimes[0]; } }
    }
}