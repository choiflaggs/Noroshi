using System.Collections.Generic;
using UniLinq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions.Roles
{
    public class Damageable
    {
        public DamageType? DamageType { get; protected set; }
        public DamageMagicalAttribute? DamageMagicalAttribute { get; protected set; }
        public TargetStateID? TargetStateID { get; protected set; }
        public uint? AttributeID { get; set; }
        public float? AttributeCoefficient { get; protected set; }
        public uint? HitCharacterEffectID { get; protected set; }


        public Damageable(
            DamageType? damageType, DamageMagicalAttribute? damageMagicalAttribute,
            TargetStateID? targetStateId,
            uint? attributeId, float? attributeCoefficient,
            uint? hitCharacterEffectId
        )
        {
            DamageType = damageType;
            DamageMagicalAttribute = damageMagicalAttribute;
            TargetStateID = targetStateId;
            AttributeID = attributeId;
            AttributeCoefficient = attributeCoefficient;
            HitCharacterEffectID = hitCharacterEffectId;
        }


        public ActionEvent[] DamageMulti(IActionExecutor executor, IEnumerable<IActionTarget> targets, int? hpDamage, int? energyDamage)
        {
            var index = 0;
            var actionEvents = new ActionEvent[targets.Count()];
            foreach (var target in new List<IActionTarget>(targets))
            {
                actionEvents[index++] = Damage(executor, target, hpDamage, energyDamage);
            }
            return actionEvents;
        }
        public ActionEvent Damage(IActionExecutor executor, IActionTarget target, int? hpDamage, int? energyDamage)
        {
            var actionEvent = new ActionEvent(executor, target);
            if (hpDamage.HasValue)
            {
                // 回避
                if (_canDodge(executor, target))
                {
                    actionEvent.SetDodge();
                    target.ReceiveActionEvent(actionEvent);
                    return actionEvent;
                }
                var damageAttribute = new DamageAttribute();
                var damageAttributeRatio = 1f;
                if (DamageMagicalAttribute.HasValue)
                {
                    damageAttributeRatio += damageAttribute.GetDamageRatio(DamageMagicalAttribute.Value, target.TagSet.GetActionTargetAttributes());
                }
                if (damageAttributeRatio != 1) GlobalContainer.Logger.Debug("Damage Attribute Ratio " + damageAttributeRatio);
                actionEvent.SetHPDamage((int)(hpDamage.Value * damageAttributeRatio), DamageType);
            }
            if (energyDamage.HasValue) actionEvent.SetEnergyDamage(energyDamage.Value);
            if (AttributeID.HasValue)
            {
                actionEvent.SetAttribute(AttributeID.Value, AttributeCoefficient.Value);
            }
            if (TargetStateID.HasValue)
            {
                actionEvent.SetTargetStateID(TargetStateID.Value);
            }
            if (HitCharacterEffectID.HasValue) actionEvent.SetHitCharacterEffectID(HitCharacterEffectID.Value);
            target.ReceiveActionEvent(actionEvent);
            return actionEvent;
        }
        
        bool _canDodge(IActionExecutor executor, IActionTarget target)
        {
            if (DamageType != Actions.DamageType.Physical) return false;
            var dodge = target.Dodge + (executor.HasMissDamage() ? 100 : 0);
            var threshold = System.Math.Max(0, dodge - executor.Accuracy) / 100f;
            return GlobalContainer.RandomGenerator.GenerateFloat() < threshold;
        }
    }
}
