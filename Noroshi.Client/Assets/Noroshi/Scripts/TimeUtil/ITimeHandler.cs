using UniRx;

namespace Noroshi.TimeUtil
{
    public interface ITimeHandler
    {
        IObservable<long> Timer(float timeSpan);
        IObservable<long> Interval(float timeSpan);
        uint UnixTime { get; }
        void SynchronizeWithServerTime(uint serverUnixTime);
    }
}
