using System;
using System.Collections.Generic;
using UniLinq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions.Roles
{
    public class Attackable : Damageable
    {
        public int DamageBoost { get; protected set; }
        float? _damageCoefficient{ get; set; }

        public Attackable(
            DamageType? damageType, DamageMagicalAttribute? damageMagicalAttribute,
            TargetStateID? targetStateId,
            uint? attributeId, float? attributeCoefficient,
            uint? hitCharacterEffectId,
            int damageBoost,
            float? damageCoefficient
        ) : base(damageType, damageMagicalAttribute, targetStateId, attributeId, attributeCoefficient, hitCharacterEffectId)
        {
            DamageBoost = damageBoost;
            _damageCoefficient = damageCoefficient;
        }
        public ActionEvent[] AttackMulti(IActionExecutor executor, IEnumerable<IActionTarget> targets, IEnumerable<int> energyDamages = null)
        {
            var index = 0;
            var actionEvents = new ActionEvent[targets.Count()];
            foreach (var target in new List<IActionTarget>(targets))
            {
                actionEvents[index] = Attack(executor, target, energyDamages != null ? (int?)energyDamages.ElementAt(index) : null);
                index++;
            }
            return actionEvents;
        }
        public ActionEvent Attack(IActionExecutor executor, IActionTarget target, int? energyDamage = null)
        {
            var actionEvent = new ActionEvent(executor, target);
            var damage = 0;
            if (DamageType == Actions.DamageType.Physical) damage = _calculatePhysicalDamage(executor, target);
            if (DamageType == Actions.DamageType.Magical ) damage = _calculateMagicDamage   (executor, target);
            actionEvent = Damage(executor, target, _calculateDamageCoefficient(damage + DamageBoost), energyDamage);
            return actionEvent;
        }

        int _calculateDamageCoefficient(int damage)
        {
            if (_damageCoefficient.HasValue)
            {
                damage = (int)(damage * _damageCoefficient.Value);
            }
            return damage;
        }

        int _calculatePhysicalDamage(IActionExecutor attacker, IActionTarget target)
        {
            var randomGenerator = GlobalContainer.RandomGenerator;
            // 基本ダメージ
            var damage = (float)(attacker.PhysicalAttack - Math.Max(0, target.Armor - attacker.ArmorPenetration));
            damage = Math.Max(0, damage);
            // クリティカル
            if (randomGenerator.GenerateFloat() < 0.5f) damage += attacker.PhysicalCrit;
            // ダメージ補正
            damage += damage * (randomGenerator.GenerateFloat() * 0.1f - 0.05f);
            return (int)damage;
        }

        int _calculateMagicDamage(IActionExecutor attacker, IActionTarget target)
        {
            var randomGenerator = GlobalContainer.RandomGenerator;
            // 基本ダメージ
            var damage = (float)(attacker.MagicPower - Math.Max(0, target.MagicRegistance - attacker.IgnoreMagicResistance));
            damage = Math.Max(0, damage);
            // クリティカル
            if (randomGenerator.GenerateFloat() < 0.5f) damage += attacker.MagicCrit;
            // ダメージ補正
            damage += damage * (randomGenerator.GenerateFloat() * 0.1f - 0.05f);
            return (int)damage;
        }
    }
}
