using System.Collections.Generic;
using UniLinq;
using System;

namespace Noroshi.BattleScene.Actions.Roles
{
    public abstract class AbstractSearchable
    {
        public abstract int? MaxTargetNum{ get; set; }
        public abstract TargetSortType? SortType { get; set; }
        
        protected IEnumerable<IActionTarget> _sort(IEnumerable<IActionTarget> targets, Force force)
        {
            switch (SortType)
            {
            case TargetSortType.CurrentHPAsc:
                return targets.OrderBy(t => t.CurrentHP);
            case TargetSortType.MaxHPDesc:
                return targets.OrderByDescending(t => t.MaxHP);
            case TargetSortType.PositionFront:
                return force == Force.Own ? 
                       targets.OrderByDescending(t => t.GetGridPosition().Value.HorizontalIndex)
                       : targets.OrderBy(t => t.GetGridPosition().Value.HorizontalIndex);
            case TargetSortType.Random:
                return targets.OrderBy(_ => GlobalContainer.RandomGenerator.GenerateFloat());
            default:
                throw new Exception("Invalid master.");
            }
        }
        
        public virtual IEnumerable<IActionTarget> FilterTargets(IActionTargetFinder actionTargetFinder, IEnumerable<IActionTarget> targets)
        {
            var map = actionTargetFinder.GetAllTargets().Where(t => t.IsTargetable).ToDictionary(t => t);
            return MaxTargetNum.HasValue ? targets.Where(t => map.ContainsKey(t)).Take(MaxTargetNum.Value) : targets.Where(t => map.ContainsKey(t));
        }
    }
}