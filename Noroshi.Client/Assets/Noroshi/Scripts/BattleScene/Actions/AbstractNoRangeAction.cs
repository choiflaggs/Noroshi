using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public abstract class AbstractNoRangeAction : AbstractAction
    {
        protected NoRangeSearchable _noRangeSearchable;
        public AbstractNoRangeAction(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _noRangeSearchable = new NoRangeSearchable()
            {
                SelectType = _targetSelectType,
                SortType = _targetSortType,
                TagIndex = _targetTagFlags,
                MaxTargetNum = _maxTargetNum,
                Trigger = Trigger,
            };
        }
        protected abstract TargetSelectType? _targetSelectType { get; }
        protected abstract int? _targetTagFlags { get; }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            return _noRangeSearchable.Search(actionTargetFinder, executor).ToArray();
        }
    }
}
