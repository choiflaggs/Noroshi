using System;
using Noroshi.Server.Entity.Debug;

namespace Noroshi.Server.Contexts
{
    public class DebugTimeHandler : TimeHandler
    {
        public DebugTimeHandler(string hostName)
        {
            var debugTime = TimeDebugEntity.CreateOrSelect(hostName);
            LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo).AddSeconds(debugTime.ModifyTime);
            UnixTime = _convertToUnixTime(LocalDateTime);
        }
    }
}
