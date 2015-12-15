using System.Collections.Generic;
using UniLinq;
using System;

namespace Noroshi.BattleScene
{
    public class StatusForceSetter
    {

        public StatusForceSetter()
        {
            Armor = null;
            MagicRegistance = null;
        }

        List<Factor> _factors = new List<Factor>();

        public int? Armor { get; private set; }
        public int? MagicRegistance { get; private set; }

        public void AddFactor(Factor factor)
        {
            if (Armor.HasValue)
            {
                throw new InvalidOperationException("StatusForceSetter : AddFactor");
            }
            if (MagicRegistance.HasValue)
            {
                throw new InvalidOperationException("StatusForceSetter : AddFactor");
            }
            _factors.Add(factor);
            _calculate();
        }

        public void RemoveFactor(Factor factor)
        {
            _factors.Remove(factor);
            _calculate();
        }

        void _calculate()
        {
            Armor = _factors.Where(f => f.Armor.HasValue).Select(f => f.Armor).FirstOrDefault();
            MagicRegistance = _factors.Where(f => f.MagicRegistance.HasValue).Select(f => f.MagicRegistance).FirstOrDefault();
        }

        public class Factor
        {
            public int? Armor { get; set; }
            public int? MagicRegistance { get; set; }
        }
    }
}