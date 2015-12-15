using System;
using Noroshi.Core.Utility;

namespace Noroshi.Core.Game.Player
{
    public class StaminaHandler : NaturalIncreaseValue
    {
        public StaminaHandler() : base(Constant.STAMINA_RECOVERY_TIME_SPAN)
        {
        }

        public ushort MaxValue(ushort playerLevel)
        {
            return (ushort)(Constant.INITIAL_MAX_STAMINA + (playerLevel - 1));
        }

        public ushort CurrentValue(ushort playerLevel, ushort lastValue, uint lastUpdatedAt, uint currentUnixTime)
        {
            return _currentValue(lastValue, lastUpdatedAt, MaxValue(playerLevel), currentUnixTime);
        }

        public TimeSpan? GetTimeToNextIncrease(ushort playerLevel, ushort lastValue, uint lastUpdatedAt, uint currentUnixTime)
        {
            return _getTimeToNextIncrease(lastValue, lastUpdatedAt, MaxValue(playerLevel), currentUnixTime);
        }
    }
}
