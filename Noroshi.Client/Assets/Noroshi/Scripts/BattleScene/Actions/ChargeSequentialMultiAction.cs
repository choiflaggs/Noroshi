using System;
using UniLinq;
using System.Collections.Generic;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class ChargeSequentialMultiAction : AbstractMultiAction
    {
        int _currentActionIndex;
        ChargeActable _chargeActable;
        RangeSearchable _rangeSearchable;
        int _minRange { get { return _arg1; } }
        int _maxRange { get { return _arg2; } }
        protected override uint? _actionId1 { get { return _arg3 > 0 ? (uint?)_arg3 : null; } }
        protected override uint? _actionId2 { get { return _arg4 > 0 ? (uint?)_arg4 : null; } }
        protected override uint? _actionId3 { get { return _arg5 > 0 ? (uint?)_arg5 : null; } }
        protected override uint? _actionId4 { get { return _arg6 > 0 ? (uint?)_arg6 : null; } }
        protected override uint? _actionId5 { get { return _arg7 > 0 ? (uint?)_arg7 : null; } }
        protected override uint? _actionId6 { get { return _arg8 > 0 ? (uint?)_arg8 : null; } }
        protected override uint? _actionId7 { get { return _arg9 > 0 ? (uint?)_arg9 : null; } }
        protected override uint? _actionId8 { get { return _arg10 > 0 ? (uint?)_arg10 : null; } }

        public ChargeSequentialMultiAction(Core.WebApi.Response.Character.Action data) : base(data)
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

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _currentActionIndex = 0;
            _chargeActable = new ChargeActable()
            {
                Force = RelativeForce.Enemy,
                FirstActionMinRange = _getActionMinRange(),
                FirstActionMaxRange = _getActionMaxRange(),
                Animation = _animation,
                TargetSortType = _targetSortType,
                MaxTargetNum = _maxTargetNum,
            };
            _chargeActable.Initialize();
            _chargeActable.PreProcess();
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _chargeActable.ExecuteAndGoBack(actionTargetFinder, executor, targets, () =>
            {
                //  登録されている分だけアクションを実行
                if (_currentActionIndex >= _actions.Length - 1) _currentActionIndex = _actions.Length - 1;
                var attackTargets = _actions[_currentActionIndex].GetTargets(actionTargetFinder, executor);
                _actions[_currentActionIndex++].Execute(actionTargetFinder, executor, attackTargets);
            });
        }
        
        int _getActionMinRange()
        {
            return ((AbstractRangeAction)_actions[_currentActionIndex]).MinRange;
        }
        
        int _getActionMaxRange()
        {
            return ((AbstractRangeAction)_actions[_currentActionIndex]).MaxRange;
        }
    }
}
