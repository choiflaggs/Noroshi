using System.Collections.Generic;
using UniLinq;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions.Roles
{
    public class RatioDamageable : Damageable
    {
        public RatioDamageable(
            TargetStateID? targetStateId,
            uint? attributeId, float? attributeCoefficient,
            uint? hitCharacterEffectId
        ) : base(null, null, targetStateId, attributeId, attributeCoefficient, hitCharacterEffectId)
        {
        }

        public void RatioDamageMulti(IActionExecutor executor, IEnumerable<IActionTarget> targets, float? hpDamageRatio, float? energyDamageRatio)
        {
            foreach (var target in new List<IActionTarget>(targets))
            {
                RatioDamage(executor, target, hpDamageRatio, energyDamageRatio);
            }
        }
        public void RatioDamage(IActionExecutor executor, IActionTarget target, float? hpDamageRatio, float? energyDamageRatio)
        {
            var hpDamage     = hpDamageRatio.HasValue ? (int?)(target.MaxHP * hpDamageRatio) : null;
            var energyDamage = energyDamageRatio.HasValue ? (int?)(target.MaxEnergy * energyDamageRatio) : null;
            Damage(executor, target, hpDamage, energyDamage);
        }
    }
}
