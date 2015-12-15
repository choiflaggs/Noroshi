using UniRx;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;
using System.Collections.Generic;

namespace Noroshi.BattleScene.Actions
{
    public abstract class AbstractWideAttack : AbstractNoRangeAttack
    {
        RangeSearchable _rangeSearchable;
        Grid.GridPosition _centerGrid;
        int _executeNum = 0;

        public AbstractWideAttack(Core.WebApi.Response.Character.Action data) : base(data)
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
        protected abstract int _attackMinRange { get; }
        protected abstract int _attackMaxRange { get; }
        protected abstract bool _isAsynchronous { get; }
        protected abstract float? _energyDamageRatio { get; }
        protected virtual byte? _executeAttributeNum { get { return null; } }
        protected virtual byte? _animationSequenceNumber { get { return null; } }
        
        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess(executor, targetCandidates);
            _tryToSetActionViewSkinLevel(executor.SkinLevel);
            _executeNum = 0;
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
            _executeNum++;
            if (_executeAttributeNum.HasValue) _attackable.AttributeID = _executeAttributeNum.Value == _executeNum ? _attributeId : null;
            var targets = _rangeSearchable.GetTargetsByRange(actionTargetFinder, _centerGrid, executor.GetDirection(), executor.CurrentForce);
            return _attackable.AttackMulti(executor, targets, _getEnergyDamage(executor, targets));
        }

        IEnumerable<int> _getEnergyDamage(IActionExecutor executor, IEnumerable<IActionTarget> targets)
        {
            if (!_energyDamageRatio.HasValue) return null;
            var energyDamageList = new List<int>();
            foreach (var target in targets)
            {
                energyDamageList.Add((int)(_energyDamageRatio.Value * target.MaxEnergy));
            }
            return energyDamageList;
        }
    }
}