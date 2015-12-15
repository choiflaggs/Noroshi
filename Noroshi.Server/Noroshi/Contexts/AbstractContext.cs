using System;

namespace Noroshi.Server.Contexts
{
    public abstract class AbstractContext : IContext
    {
        public Logger Logger { get; private set; }
        public Cache Cache { get; private set; }
        public IRandomGenerator RandomGenerator { get; private set; }
        public ITimeHandler TimeHandler { get; private set; }
        public uint? ShardID { get; protected set; }

        public AbstractContext()
        {
            Logger = new Logger();
            Cache = new Cache();
            TimeHandler = new TimeHandler();
            RandomGenerator = new RandomGenerator();
        }

        public void Initialize()
        {
#if DEBUG
            TimeHandler = new DebugTimeHandler(Environment.MachineName);
#endif
        }

        public void ClearRdbConnections()
        {
            Cache.ClearRdbConnections();
        }

        public void Dispose()
        {
            Cache.Dispose();
        }
    }
}
