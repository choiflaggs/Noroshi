using Noroshi.BattleScene.Actions.Roles;
using System;
using UniLinq;

namespace Noroshi.BattleScene.Actions
{
    public class DelayAppearanceAttack : AbstractNoRangeAttack 
    {
        RangeSearchable _rangeSearchable;
        int _executeNum;

        public DelayAppearanceAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _rangeSearchable = new RangeSearchable()
            {
                Force = RelativeForce.Enemy,
                MinRange = _attackMinRange,
                MaxRange = _attackMaxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = _targetSortType,
            };
        }

        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        int _attackMinRange { get { return _arg4; } }
        int _attackMaxRange { get { return _arg5; } }

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
                var targetHorizontalIndex = _getTargetHorizontalIndex(executor, GetTargets(actionTargetFinder, executor));
                _setStartPosition(executor, targetHorizontalIndex);
                var horizontalIndex = _getHorizontalIndex(executor, targetHorizontalIndex);
                executor.HorizontalMove(horizontalIndex, _forwardDuration);
            }
            else if (_executeNum == 2)
            {
                // 何もしない
            }
            else
            {
                var attackTargets = _rangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce);
                _attackable.AttackMulti(executor, attackTargets);
            }
        }

        int _getTargetHorizontalIndex(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            var originalhorizontalIndex = executor.GetGridPosition().Value.HorizontalIndex;
            return targetCandidates.Count() > 0 ? (int)targetCandidates.Average(t => t.GetGridPosition().Value.HorizontalIndex) : originalhorizontalIndex;
        }

        short _getHorizontalIndex(IActionExecutor executor, int horizontalIndex)
        {
            var k = executor.GetDirection() == Noroshi.Grid.Direction.Left ? -1 : 1;
            var originalHorizontalIndex = executor.GetGridPosition().Value.HorizontalIndex;
            return (short)((horizontalIndex - originalHorizontalIndex) * k - (_attackMinRange + _attackMaxRange) / 2);
        }

        void _setStartPosition(IActionExecutor executor, int horizontalIndex)
        {
            var h = Constant.VISIBLE_FIELD_HORIZONTAL_GRID_SIZE;
            var startHorizontalIndex = (short)(horizontalIndex + ((executor.CurrentForce == Force.Own) ? -h : h));
            executor.HorizontalMove(startHorizontalIndex, 0);
        }

        float _forwardDuration { get { return _animation.TriggerTimes[1] - _animation.TriggerTimes[0]; } }
    }
}