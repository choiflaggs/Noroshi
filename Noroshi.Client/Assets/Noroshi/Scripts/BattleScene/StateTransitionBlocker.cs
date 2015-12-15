using System.Collections.Generic;
using UniLinq;

namespace Noroshi.BattleScene
{
    public class StateTransitionBlocker
    {
        List<Factor> _factors = new List<Factor>();

        public StateTransitionBlocker()
        {
        }

        public bool ActiveActionBlock { get; private set; }
        public bool MagicActionBlock { get; private set; }
        public bool ProceedBlock { get; private set; }

        public void AddFactor(Factor factor)
        {
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
            ActiveActionBlock = _factors.Any(f => f.ActiveActionBlock);
            MagicActionBlock  = _factors.Any(f => f.MagicActionBlock);
            ProceedBlock      = _factors.Any(f => f.ProceedBlock);
        }

        public class Factor
        {
            public bool ActiveActionBlock { get; set; }
            public bool MagicActionBlock { get; set; }
            public bool ProceedBlock { get; set; }
        }
    }
}
