namespace Noroshi.Server.Contexts
{
    public class CliContext : AbstractContext
    {
        public CliContext(uint shardId)
        {
            ShardID = shardId;
        }
    }
}
