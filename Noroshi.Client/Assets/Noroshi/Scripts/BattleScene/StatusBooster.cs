using System.Collections.Generic;
using UniLinq;
using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene
{
    public class StatusBooster
    {
        List<IStatusBoostFactor> _factors = new List<IStatusBoostFactor>();

        public StatusBooster()
        {
            Strength        = 0;
            Intellect       = 0;
            Agility         = 0;
            PhysicalAttack  = 0;
            MagicPower      = 0;
            Armor           = 0;
            MagicRegistance = 0;
            Accuracy        = 0;
            Dodge           = 0;
            LifeStealRating = 0;
            ActionFrequency = 0;
            MaxHp           = 0;
        }

        public float Strength { get; private set; }
        public float Intellect { get; private set; }
        public float Agility { get; private set; }
        public int PhysicalAttack { get; private set; }
        public int MagicPower { get; private set; }
        public int Armor { get; private set; }
        public int MagicRegistance { get; private set; }
        public sbyte Accuracy { get; private set; }
        public sbyte Dodge { get; private set; }
        public int LifeStealRating { get; private set; }
        public float ActionFrequency { get; private set; }
        public int MaxHp { get; private set; } 

        public void AddFactor(IStatusBoostFactor factor)
        {
            _factors.Add(factor);
            _calculate();
        }
        public void AddFactors(IEnumerable<IStatusBoostFactor> factors)
        {
            _factors.AddRange(factors);
            _calculate();
        }
        public void RemoveFactor(IStatusBoostFactor factor)
        {
            _factors.Remove(factor);
            _calculate();
        }

        void _calculate()
        {
            Strength        = _factors.Sum(f => f.Strength);
            Intellect       = _factors.Sum(f => f.Intellect);
            Agility         = _factors.Sum(f => f.Agility);
            PhysicalAttack  = _factors.Sum(f => f.PhysicalAttack );
            MagicPower      = _factors.Sum(f => f.MagicPower);
            Armor           = _factors.Sum(f => f.Armor);
            MagicRegistance = _factors.Sum(f => f.MagicRegistance);
            Accuracy        = (sbyte)_factors.Sum(f => f.Accuracy);
            Dodge           = (sbyte)_factors.Sum(f => f.Dodge);
            LifeStealRating = _factors.Sum(f => f.LifeStealRating);
            ActionFrequency = _factors.Sum(f => f.ActionFrequency);
            MaxHp           = _factors.Sum(f => f.MaxHp);
        }
    }
}
