using System;
using Noroshi.Core.Utility;

namespace Noroshi.Core.Game.Player
{
    public class ActionLevelPointHandler : NaturalIncreaseValue
    {
        public ActionLevelPointHandler() : base(Constant.ACTION_LEVEL_POINT_RECOVERY_TIME_SPAN)
        {
        }

        public ushort CurrentValue(byte lastValue, uint lastUpdatedAt, byte maxNum, uint currentUnixTime)
        {
            return _currentValue(lastValue, lastUpdatedAt, maxNum, currentUnixTime);
        }

        public TimeSpan? GetTimeToNextIncrease(byte lastValue, uint lastUpdatedAt, byte maxNum, uint currentUnixTime)
        {
            return _getTimeToNextIncrease(lastValue, lastUpdatedAt, maxNum, currentUnixTime);
        }
    }
}
