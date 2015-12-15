using System.Collections.Generic;
using System;
using UniRx;

namespace Noroshi.BattleScene.Actions.Roles
{
    public class ChargeAttackable : ChargeActable
    {
        Attackable _attackable;
        public float? DamageCoefficient { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            _attackable = new Attackable(
                DamageType, DamageMagicalAttribute,
                TargetStateID,
                AttributeID, AttributeCoefficient,
                HitCharacterEffectID,
                DamageBoost,
                DamageCoefficient
            );
        }

        void _attack(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            var attackTargets = _attackRangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce);
            _attackable.AttackMulti(executor, attackTargets);
        }
        
        void _attack(IActionTargetFinder actionTargetFinder, IActionExecutor executor, System.Action onAttack)
        {
            var attackTargets = _attackRangeSearchable.GetTargetsByRange(actionTargetFinder, executor.GetGridPosition().Value, executor.GetDirection(), executor.CurrentForce);
            _attackable.AttackMulti(executor, attackTargets);
            if (onAttack != null) onAttack();
        }

        public void ExecuteAndGoBack(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets)
        {
            base.ExecuteAndGoBack(actionTargetFinder, executor, targets, () => 
            {
                _attack(actionTargetFinder, executor);
            });
        }
        
        public new void ExecuteAndGoBack(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets, System.Action onAttack)
        {
            base.ExecuteAndGoBack(actionTargetFinder, executor, targets, () => 
            {
                _attack(actionTargetFinder, executor, onAttack);
            });
        }

        public void ExecuteAndGoForward(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets)
        {
            base.ExecuteAndGoForward(actionTargetFinder, executor, targets, () => 
            {
                _attack(actionTargetFinder, executor);
            });
        }
        
        public new void ExecuteAndGoForward(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets, System.Action onAttack)
        {
            base.ExecuteAndGoForward(actionTargetFinder, executor, targets, () => 
            {
                _attack(actionTargetFinder, executor, onAttack);
            });
        }
    }
}
