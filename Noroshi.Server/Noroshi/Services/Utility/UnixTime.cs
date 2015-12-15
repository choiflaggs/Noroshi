using System;

namespace Noroshi.Server.Services.Utility
{
    public class UnixTime
    {
        // unix epochをDateTimeで表した定数
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // DateTimeをUNIX時間に変換するメソッド
        public static uint ToUnixTime(DateTime dateTime)
        {
            // 時刻をUTCに変換
            dateTime = dateTime.ToUniversalTime();

            // unix epochからの経過秒数を求める
            return (uint) dateTime.Subtract(UnixEpoch).TotalSeconds;
        }

        // UNIX時間からDateTimeに変換するメソッド
        public static DateTime FromUnixTime(uint unixTime)
        {
            // unix epochからunixTime秒だけ経過した時刻を求める
            return UnixEpoch.AddSeconds(unixTime);
        }
    }
}