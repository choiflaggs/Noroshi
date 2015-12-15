using Noroshi.BattleScene.Actions.Roles;
using System;
using UniLinq;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions
{
    public class ChargeDefenseAttack : AbstractNoRangeAttack
    {
        Attackable _firstAttackable = null;
        Attackable _secondAttackable = null;
        RangeSearchable _attackRangeSearchable = null;
        ActionEvent _actionEvent = null;
        int _executeNum = 0;
        int _originalHorizontalIndex = 0;
        public ChargeDefenseAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
            _firstAttackable = new Attackable(
                DamageType, _damageMagicalAttribute,
                _targetStateId,
                null, 0.0f,
                _hitCharacterEffectID,
                _getFirstActionDamageBoost(),
                _damageCoefficient
            );
            _secondAttackable = new Attackable(
                DamageType, _damageMagicalAttribute,
                TargetStateID.Damage,
                null, 0.0f,
                _hitCharacterEffectID,
                _getSecondActionDamageBoost(),
                _damageCoefficient
            );
        }

        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg3 > 0 ? (float?)_arg3 / 100.0f : null; } }
        protected int _firstAttackMinRange { get { return _arg4; } }
        protected int _firstAttackMaxRange { get { return _arg5; } }
        protected int _secondAttackMinRange { get { return _arg6; } }
        protected int _secondAttackMaxRange { get { return _arg7; } }

        public override void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates)
        {
            base.PreProcess (executor, targetCandidates);
            _executeNum = 0;
            _attackRangeSearchable = new RangeSearchable()
            {
                Force = RelativeForce.Enemy,
                MinRange = _firstAttackMinRange,
                MaxRange = _firstAttackMaxRange,
                MaxTargetNum = _maxTargetNum,
                SortType = _targetSortType,
            };
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            _executeNum++;
            if (_executeNum == 1)
            {
                // Front Move Start
                var k = executor.GetDirection() == Grid.Direction.Left ? -1 : 1;
                _originalHorizontalIndex = executor.GetGridPosition().Value.HorizontalIndex;
                Func<short> getHorizontalIndex = () => {
                    var filterTargets = _noRangeSearchable.FilterTargets(actionTargetFinder, targets);
                    var toH = filterTargets.Count() > 0 ? (int)filterTargets.Average(t => t.GetGridPosition().Value.HorizontalIndex) : _originalHorizontalIndex;
                    return (short)((toH - _originalHorizontalIndex) * k - (_firstAttackMinRange + _firstAttackMaxRange) / 2);
                };
                if (getHorizontalIndex() != 0) executor.HorizontalMove(getHorizontalIndex, _forwardDuration);
            }
            else if (_executeNum == 2)
            {
                // Front Move End
            }
            else if (_executeNum == _animation.TriggerTimes.Length - 3)
            {
                // First Action
                var attackTargets = _attackRangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce);
                _firstAttackable.AttackMulti(executor, attackTargets);
                if (_attributeId.HasValue)
                {
                    _actionEvent = new ActionEvent(executor, actionTargetFinder.GetExecutorAsTarget(executor));
                    _actionEvent.SetAttribute(_attributeId.Value, _getLevelDrivenAttributeParam());
                    actionTargetFinder.GetExecutorAsTarget(executor).ReceiveActionEvent(_actionEvent);
                }
            }
            else if (_executeNum == _animation.TriggerTimes.Length - 2)
            {
                // Second Action
                if (_attributeId.HasValue)
                {
                    _actionEvent.SetForceRemoveAttribute();
                    actionTargetFinder.GetExecutorAsTarget(executor).ReceiveActionEvent(_actionEvent);
                }
                _attackRangeSearchable.MinRange = _secondAttackMinRange;
                _attackRangeSearchable.MaxRange = _secondAttackMaxRange;
                _attackRangeSearchable.MaxTargetNum = 1;
                var attackTargets = _attackRangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce);
                _secondAttackable.AttackMulti(executor, attackTargets);
            }
            else if (_executeNum == _animation.TriggerTimes.Length - 1)
            {
                // Back Move Start
                var h = _originalHorizontalIndex - executor.GetGridPosition().Value.HorizontalIndex;
                if (executor.GetDirection() == Grid.Direction.Left) h *= -1;
                if (h != 0) executor.HorizontalMove((short)h, _backwardDuration);
            }
            else if (_executeNum == _animation.TriggerTimes.Length)
            {
                // Back Move End
            }
        }
        // ノックバック時のダメージ
        int _getFirstActionDamageBoost()
        {
            return (int)_getLevelDrivenParam1();
        }
        // 防御成功後のダメージ
        int _getSecondActionDamageBoost()
        {
            return (int)_getLevelDrivenParam2();
        }

        float _forwardDuration { get { return _animation.TriggerTimes[1] - _animation.TriggerTimes[0]; } }
        float _backwardDuration { get { return _animation.TriggerTimes[_animation.TriggerTimes.Length - 1] - _animation.TriggerTimes[_animation.TriggerTimes.Length - 2]; } }
    }
}