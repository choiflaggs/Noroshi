using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class ShadowMaker : AbstractAction
    {
        NoRangeSearchable _noRangeSearchable;
        Queue<IShadow> _queue = new Queue<IShadow>();

        public ShadowMaker(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _noRangeSearchable = new NoRangeSearchable()
            {
                SelectType = _targetSelectType,
                SortType = _targetSortType,
                TagIndex = null,
                MaxTargetNum = 1,
                Trigger = Trigger,
            };
        }

        uint _shadowCharacterId { get { return (uint)_arg1; } }
        TargetSelectType? _targetSelectType { get { return _arg2 > 0 ? (TargetSelectType?)_arg2 : null; } }
        short _executorMoveHorizontalDistance { get { return (short)_arg3; } }
        ushort _actionLevelBaseValue2 { get { return (ushort)_arg4; } }
        ushort _actionLevelBaseValue3 { get { return (ushort)_arg5; } }

        public override IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory)
        {
            for (var i = 0; i < _maxShadowNum; i++)
            {
                _queue.Enqueue(executor.BuildShadow(_shadowCharacterId));
            }
            return Observable.WhenAll(_queue.Select(s => s.LoadDatasAndAssets(factory)))
            .Select(_ => (IAction)this);
        }

        public override IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            // 対象分身キャラクターがすでに画面上に出現している場合はアクションをスキップさせる
            if (_queue.Count <= 0) return new IActionTarget[]{};
            return new IActionTarget[]{actionTargetFinder.GetExecutorAsTarget(executor)};
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            var target = actionTargetFinder.GetExecutorAsTarget(executor);
            var appearTargets = _noRangeSearchable.Search(actionTargetFinder, executor);
            var horizontalIndex = appearTargets.Count() > 0 ? (ushort)appearTargets.Average(t => t.GetGridPosition().Value.HorizontalIndex) : target.GetGridPosition().Value.HorizontalIndex;
            var shadow = _queue.Dequeue();
            target.MakeShadow(shadow, _getShadowLevel(), _getActionLevel2(), _getActionLevel3(), horizontalIndex).Subscribe(s => _queue.Enqueue(s));
            if (_executorMoveHorizontalDistance != 0) executor.HorizontalMove(_executorMoveHorizontalDistance, _executorMoveDuration);
        }

        ushort _getShadowLevel()
        {
            return (ushort)_getLevelDrivenParam1();
        }
        ushort? _getActionLevel2()
        {
            var level = _actionLevelBaseValue2 * _getLevelDrivenParam2();
            return level > 0 ? (ushort?)level : null;
        }
        ushort? _getActionLevel3()
        {
            var level = _actionLevelBaseValue3 * _getLevelDrivenParam3();
            return level > 0 ? (ushort?)level : null;
        }

        float _executorMoveDuration { get { return 1f; } }
        int _maxShadowNum { get { return 1; } }
    }
}
