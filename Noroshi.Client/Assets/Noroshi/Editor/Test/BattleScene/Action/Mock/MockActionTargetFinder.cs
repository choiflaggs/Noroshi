using System;
using System.Collections.Generic;
using UniLinq;
using NSubstitute;
using Noroshi.Grid;
using Noroshi.BattleScene;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.Editor.Test.BattleScene.Action.Mock
{
    public class MockActionTargetFinder : IActionTargetFinder
    {
        const int HORIZONTAL_SIZE = Constant.FIELD_HORIZONTAL_GRID_SIZE;
        IActionTarget _actionExecutor;
        List<IActionTarget> _actionTargets = new List<IActionTarget>();

        public IActionExecutor SetExecutor(int h, IActionExecutor executor, Force force)
        {
            executor.Force.Returns(force);
            executor.GetGridPosition().Returns(new GridPosition((ushort)h, 0));
            
            var target = Substitute.For<IActionTarget>();
            SetTarget(h, target, force);
            _actionExecutor = target;
            
            return executor;
        }
        
        public void SetTarget(int h, IActionTarget target, Force force)
        {
            target.Force.Returns(force);
            target.GetGridPosition().Returns(new GridPosition((ushort)h, 0));
            target.IsTargetable.Returns(true);
            _actionTargets.Add(target);
        }

        public byte GetNoByActionTarge(IActionTarget actionTarget)
        {
            return _actionTargets.Select((at, i) => i)
                .Where(i => _actionTargets[i] == actionTarget).Select(i => (byte)(i + 1)).First();
        }

        public void ClearTargets()
        {
            _actionTargets.Clear();
            _actionExecutor = null;
        }

        public IActionTarget GetExecutorAsTarget(IActionExecutor executor)
        {
            return _actionExecutor;
        }
        public IEnumerable<IActionTarget> GetAllTargets()
        {
            return new List<IActionTarget>(_actionTargets);
        }
        public IEnumerable<IActionTarget> GetTargetsWithHorizontalRange(GridPosition baseGridPosition, Direction horizontalDirection, int minRange, int maxRange)
        {
            var minIndex = 0;
            var maxIndex = 0;
            switch (horizontalDirection)
            {
            case Direction.Right:
                minIndex = baseGridPosition.HorizontalIndex + minRange;
                maxIndex = baseGridPosition.HorizontalIndex + maxRange;
                break;
            case Direction.Left:
                minIndex = baseGridPosition.HorizontalIndex - maxRange;
                maxIndex = baseGridPosition.HorizontalIndex - minRange;
                break;
            default:
                throw new InvalidOperationException();
            }
            if (minIndex < 0) minIndex = 0;
            if (HORIZONTAL_SIZE - 1 < maxIndex) maxIndex = HORIZONTAL_SIZE - 1;
            
            return GetAllTargets().Where(t => minIndex <= t.GetGridPosition().Value.HorizontalIndex && t.GetGridPosition().Value.HorizontalIndex <= maxIndex);
        }
    }
}
