using System;
using UniRx;

namespace Noroshi.TimeUtil
{
    public class TimeHandler : ITimeHandler
    {
        static readonly DateTime UNIX_TIME_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public IObservable<long> Timer(float dueTime)
        {
            return Observable.Timer(TimeSpan.FromSeconds(dueTime));
        }
        public IObservable<long> Interval(float timeSpan)
        {
            return Observable.Interval(TimeSpan.FromSeconds(timeSpan));
        }

        public uint UnixTime
        {
            get
            {
                return (uint)((DateTime.Now.ToUniversalTime() - UNIX_TIME_START).TotalSeconds);
            }
        }

        public void SynchronizeWithServerTime(uint serverUnixTime)
        {
        }
    }
}
