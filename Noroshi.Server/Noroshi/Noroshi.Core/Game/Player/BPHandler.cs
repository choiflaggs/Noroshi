using System;
using Noroshi.Core.Utility;

namespace Noroshi.Core.Game.Player
{
    public class BPHandler : NaturalIncreaseValue
    {
        public BPHandler() : base(Constant.BP_RECOVERY_TIME_SPAN)
        {
        }

        public byte MaxValue()
        {
            return Constant.MAX_BP;
        }

        public byte CurrentValue(byte lastValue, uint lastUpdatedAt, uint currentUnixTime)
        {
            return _currentValue(lastValue, lastUpdatedAt, MaxValue(), currentUnixTime);
        }

        public TimeSpan? GetTimeToNextIncrease(byte lastValue, uint lastUpdatedAt, uint currentUnixTime)
        {
            return _getTimeToNextIncrease(lastValue, lastUpdatedAt, MaxValue(), currentUnixTime);
        }
    }
}
