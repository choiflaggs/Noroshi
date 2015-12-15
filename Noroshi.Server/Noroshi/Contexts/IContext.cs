using System;

namespace Noroshi.Server.Contexts
{
    public interface IContext : IDisposable
    {
        Logger Logger { get; }
        Cache Cache { get; }
        ITimeHandler TimeHandler { get; }
        IRandomGenerator RandomGenerator { get; }
        uint? ShardID { get; }
        void Initialize();
        void ClearRdbConnections();
    }
}
