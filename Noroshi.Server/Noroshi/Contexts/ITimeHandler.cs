using System;

namespace Noroshi.Server.Contexts
{
    public interface ITimeHandler
    {
        /// <summary>
        /// UNIX 時間を取得。
        /// </summary>
        uint UnixTime { get; }
        uint DayStartUnixTimeInUTC { get; }
        /// <summary>
        /// ローカルタイムを取得。複数リージョン対応のため、表示以外に利用するのは禁止。
        /// </summary>
        DateTime LocalDateTime { get; }
        DayOfWeek LocalDayOfWeek { get; }

        bool HasAlreadyReset(uint targetUnixTime);
        bool HasAlreadyReset(uint targetUnixTime, byte[] resetHours);
        uint GetNextResetUnixTime(uint unixtime);
        DateTime SelectNextResetDateTime(byte[] resetHours);
    }
}
