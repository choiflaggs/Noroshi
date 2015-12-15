using UniLinq;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class BehindAttack : AbstractNoRangeAttack
    {
        int _executeNum;

        public BehindAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }

        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected int _attackMinRange { get { return _arg4; } }
        protected int _attackMaxRange { get { return _arg5; } }

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
                var h = _getHorizontailIndex(actionTargetFinder, executor);
                if (h > 0) executor.HorizontalMove(h, _forwardDuration);
            }
            else if (_executeNum == 2)
            {
                // 何もしない
            }
            else if (_executeNum == 3)
            {
                executor.TryToChangeDirection();
            }
            else
            {
                _attackable.AttackMulti(executor, targets);
            }
        }

        short _getHorizontailIndex(IActionTargetFinder actionTargetFinder,  IActionExecutor executor)
        {
            var k = executor.GetDirection() == Grid.Direction.Left ? -1 : 1;
            var originalHorizontalIndex = executor.GetGridPosition().Value.HorizontalIndex;
            var filterTargets = GetTargets(actionTargetFinder, executor);
            var toH = filterTargets.Count() > 0 ? (int)filterTargets.Average(t => t.GetGridPosition().Value.HorizontalIndex) : originalHorizontalIndex;
            return (short)((toH - originalHorizontalIndex) * k + (_attackMinRange + _attackMaxRange) / 2);
        }

        float _forwardDuration { get { return _animation.TriggerTimes[1] - _animation.TriggerTimes[0]; } }
    }
}