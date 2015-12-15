using UniRx;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class StraightRangeAttack : AbstractNoRangeAttack
    {
        int _executeNum = 0;
        RangeSearchable _rangeSearchable = null;

        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg4 / 100.0f : null; } }
        protected int _attackMinRange { get { return _arg4; } }
        protected int _attakcMaxRange { get { return _arg5; } }

        public StraightRangeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _rangeSearchable = new RangeSearchable()
            {
                Force = RelativeForce.Enemy,
                MinRange = _attackMinRange,
                MaxRange = _attakcMaxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = null,
            };
        }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess (executor, targetCandidates);
            _executeNum = 0;
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _executeNum++;
            if (_executeNum == 1)
            {
                executor.GoStraight(_duration);
            }
            else
            {
                var rangeTargets = _rangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value,
                                                                      executor.GetDirection(), executor.CurrentForce);
                _attackable.AttackMulti(executor, rangeTargets);
            }
        }

        public override void PostProcess(IActionExecutor executor)
        {
            base.PostProcess(executor);
            executor.Appear();
        }
        
        float _duration { get { return _animation.FinishTime - _animation.TriggerTimes[0]; } }
    }
}
