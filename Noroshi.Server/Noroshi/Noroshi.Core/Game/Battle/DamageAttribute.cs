using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;

namespace Noroshi.Core.Game.Battle
{
    public class DamageAttribute
    {
        const float STRONG_RATIO = 0.3f;
        const float WEAK_RATIO = -0.3f;
        const float REGISTANCE_RATIO = -1f;
        static Dictionary<DamageMagicalAttribute, Dictionary<ActionTargetAttribute, float>> _magicalDamageRatios = new Dictionary<DamageMagicalAttribute, Dictionary<ActionTargetAttribute, float>>()
        {
            {DamageMagicalAttribute.Fire, new Dictionary<ActionTargetAttribute, float>(){
                {ActionTargetAttribute.Water, WEAK_RATIO},
                {ActionTargetAttribute.Wind, WEAK_RATIO},
            }},
            {DamageMagicalAttribute.Water, new Dictionary<ActionTargetAttribute, float>(){
                {ActionTargetAttribute.Fire, STRONG_RATIO},
                {ActionTargetAttribute.Thunder, WEAK_RATIO},
            }},
            {DamageMagicalAttribute.Thunder, new Dictionary<ActionTargetAttribute, float>(){
                {ActionTargetAttribute.Water, STRONG_RATIO},
                {ActionTargetAttribute.Earth, WEAK_RATIO},
                {ActionTargetAttribute.Machine, STRONG_RATIO},
            }},
            {DamageMagicalAttribute.Earth, new Dictionary<ActionTargetAttribute, float>(){
                {ActionTargetAttribute.Thunder, STRONG_RATIO},
                {ActionTargetAttribute.Wind, WEAK_RATIO},
                {ActionTargetAttribute.Flying, REGISTANCE_RATIO},
            }},
            {DamageMagicalAttribute.Wind, new Dictionary<ActionTargetAttribute, float>(){
                {ActionTargetAttribute.Fire, STRONG_RATIO},
                {ActionTargetAttribute.Earth, STRONG_RATIO},
                {ActionTargetAttribute.Flying, STRONG_RATIO},
            }},
        };
        public float GetDamageRatio(DamageMagicalAttribute magicalAttribute, IEnumerable<ActionTargetAttribute> targetAttributes)
        {
            if (!_magicalDamageRatios.ContainsKey(magicalAttribute))
            {
                return 0f;
            }
            var map = _magicalDamageRatios[magicalAttribute];
            var ratio = 0f;
            foreach (var targetAttribute in targetAttributes.Where(targetAttribute => map.ContainsKey(targetAttribute)))
            {
                ratio += map[targetAttribute];
            }
            return ratio;
        }
    }
}
