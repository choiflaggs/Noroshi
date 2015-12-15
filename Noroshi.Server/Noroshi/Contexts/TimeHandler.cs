using System;
using System.Linq;
using Noroshi.Core.Game;

namespace Noroshi.Server.Contexts
{
    public class TimeHandler : ITimeHandler
    {
        static readonly DateTime UNIX_TIME_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected static uint _convertToUnixTime(DateTime dateTime)
        {
            var elapsedTime = dateTime.ToUniversalTime() - UNIX_TIME_START;
            return (uint)elapsedTime.TotalSeconds;
        }
        protected static DateTime _convertToLocalTime(uint unixTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(UNIX_TIME_START.AddSeconds(unixTime), timeZone);
        }

        // TODO : 外からセットできるようにする。
        protected TimeZoneInfo _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        public TimeHandler()
        {
            LocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
            UnixTime = _convertToUnixTime(LocalDateTime);
        }

        public uint UnixTime { get; protected set; }
        public DateTime LocalDateTime { get; protected set; }

        public DayOfWeek LocalDayOfWeek => LocalDateTime.AddHours(-Constant.DAILY_RESET_HOUR).DayOfWeek;

        public uint DayStartUnixTimeInUTC
        {
            get
            {
                var dt = UNIX_TIME_START.AddSeconds(UnixTime);
                var dayStart = new DateTime(dt.Year, dt.Month, dt.Day);
                return _convertToUnixTime(dayStart);
            }
        }

        public bool HasAlreadyReset(uint targetUnixTime)
        {
            return HasAlreadyReset(targetUnixTime, new byte[] { Constant.DAILY_RESET_HOUR });
        }
        public bool HasAlreadyReset(uint targetUnixTime, byte[] resetHours)
        {
            var todayStart = _convertToUnixTime(LocalDateTime.Date);
            var diffDays = new sbyte[] { -1, 0 };
            return diffDays.SelectMany(diffDay => resetHours.Select(resetHour => (uint)(todayStart + (24 * diffDay + resetHour) * 3600)))
                .Any(resetUnixTime => targetUnixTime < resetUnixTime && resetUnixTime <= UnixTime);
        }
        public uint GetNextResetUnixTime(uint unixTime)
        {
            var dayStart = _convertToUnixTime(_convertToLocalTime(unixTime, _timeZoneInfo).Date);
            var diffDays = new byte[] { 0, 1 };
            return diffDays.Select(diffDay => (uint)(dayStart + (24 * diffDay + Constant.DAILY_RESET_HOUR) * 3600))
                .Where(resetUnixTime => unixTime < resetUnixTime)
                .OrderBy(resetUnixTime => resetUnixTime)
                .First();
        }

        public DateTime SelectNextResetDateTime(byte[] resetHours)
        {
            var todayStart = _convertToUnixTime(LocalDateTime.Date);
            var diffDays = new byte[] { 0, 1 };
            var nextResetUnixTime = diffDays.SelectMany(diffDay => resetHours.Select(resetHour => (uint)(todayStart + (24 * diffDay + resetHour) * 3600)))
                .Where(resetUnixTime => UnixTime < resetUnixTime)
                .OrderBy(resetUnixTime => resetUnixTime)
                .First();
            return _convertToLocalTime(nextResetUnixTime, _timeZoneInfo);
        }
    }
}
