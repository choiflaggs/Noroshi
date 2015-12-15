using System;
using System.Collections.Generic;
using UniLinq;
using Noroshi.Grid;

namespace Noroshi.BattleScene.Actions.Roles
{
    public enum RelativeForce
    {
        Own = 1,
        Enemy = 2,
    }
    public class RangeSearchable : AbstractSearchable
    {
        public RelativeForce? Force { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public override int? MaxTargetNum { get; set; }
        public override TargetSortType? SortType { get; set; }

        public IEnumerable<IActionTarget> GetTargetsByRange(IActionTargetFinder actionTargetFinder, GridPosition gridPosition, Direction direction, Force force)
        {
            var targets = actionTargetFinder.GetTargetsWithHorizontalRange(gridPosition, direction, MinRange, MaxRange)
            .Where(t =>
            {
                if (Force.HasValue)
                {
                    if (Force == RelativeForce.Own)
                    {
                        return t.Force == force;
                    }
                    else if (Force == RelativeForce.Enemy)
                    {
                        return t.Force != force;
                    }
                    else
                    {
                        throw new Exception("Invalid master.");
                    }
                }
                else
                {
                    return true;
                }
            });

            // ソート
            if (SortType.HasValue)
            {
                targets = _sort(targets, force);
            }
            else
            {
                targets = targets.OrderBy(t => System.Math.Abs(t.GetGridPosition().Value.HorizontalIndex - gridPosition.HorizontalIndex));
            }

            return FilterTargets(actionTargetFinder, targets);
        }

        public override IEnumerable<IActionTarget> FilterTargets(IActionTargetFinder actionTargetFinder, IEnumerable<IActionTarget> targets)
        {
            return base.FilterTargets(actionTargetFinder, targets);
        }
    }
}
