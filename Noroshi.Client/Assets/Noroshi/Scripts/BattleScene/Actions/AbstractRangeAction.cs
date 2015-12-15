using System.Collections.Generic;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public abstract class AbstractRangeAction : AbstractAction
    {
        protected RangeSearchable _rangeSearchable;
        public AbstractRangeAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _rangeSearchable = new RangeSearchable()
            {
                Force = _targetRelativeForce,
                MinRange = MinRange,
                MaxRange = MaxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = _targetSortType,
            };
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _rangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce).ToArray();
        }

        protected abstract RelativeForce? _targetRelativeForce { get; }
        public abstract int MinRange { get; }
        public abstract int MaxRange { get; }

        protected IEnumerable<IActionTarget> _filterTargets(IEnumerable<IActionTarget> targets)
        {
            return _maxTargetNum.HasValue ? targets.Where(t => t.IsTargetable).Take(_maxTargetNum.Value) : targets.Where(t => t.IsTargetable);
        }
    }
}