using UniRx;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class RangeWideAttack : AbstractRangeAttack
    {
        RangeSearchable _attackRangeSearchable;
        Grid.GridPosition _centerGrid;

        public RangeWideAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _attackRangeSearchable = new RangeSearchable()
            {
                Force = _targetRelativeForce,
                MinRange = _attackMinRange,
                MaxRange = _attackMaxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = _targetSortType,
            };
        }
        
        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        protected override float? _damageCoefficient { get { return _arg4 > 0 ? (float?)_arg4 / 100.0f : null; } }
        int _attackMinRange { get { return _arg5; } }
        int _attackMaxRange { get { return _arg6; } }
        bool _isAsynchronous { get { return _arg7 > 0; } }
        // TODO
        // 現状WideAttack系はActionViewしか使用していないので一旦固定値で、カメラ揺れなど必要になった場合対応予定
        protected override ActionEffectType? _actionEffectType { get { return ActionEffectType.ViewOnly; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _tryToSetActionViewSkinLevel(executor.SkinLevel);
            // 同期モードではアクション開始時に攻撃の中心を決定。
            if (!_isAsynchronous)
            {
                _setCenterGrid(targetCandidates);
                // TODO
                _tryActionViewAppearance(_centerGrid, executor.GetDirection(), AnimationName, 1000);
            }
        }
        
        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            // 非同期モードではこのタイミングで攻撃の中心を決定して View を発射。
            if (_isAsynchronous)
            {
                var targets = GetTargets(actionTargetFinder, executor);
                if (targets.Count() > 0)
                {
                    _setCenterGrid(targets);
                }
                else
                {
                    // TODO
                    _centerGrid = executor.GetGridPosition().Value;
                }
                _tryToLaunchActionView(executor.GetViewAsActionExecutorView(), _centerGrid, executor.GetDirection(), AnimationName, 1000).Subscribe(_ => 
                {
                    _attack(actionTargetFinder, executor);
                });
            }
            // 同期モードではこのタイミングで攻撃実行。
            else
            {
                _attack(actionTargetFinder, executor);
            }
        }
        
        public override void PostProcess(IActionExecutor executor)
        {
            if (!_isAsynchronous) _tryActionViewDisappearance();
        }
        
        void _setCenterGrid(IActionTarget[] targets)
        {
            var h = (ushort)targets.Average(t => t.GetGridPosition().Value.HorizontalIndex);
            var v = (ushort)targets.Average(t => t.GetGridPosition().Value.VerticalIndex);
            _centerGrid = new Grid.GridPosition(h, v);
        }
        
        protected ActionEvent[] _attack(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var targets = _attackRangeSearchable.GetTargetsByRange(actionTargetFinder, _centerGrid, executor.GetDirection(), executor.CurrentForce);
            return _attackable.AttackMulti(executor, targets);
        }
    }
}