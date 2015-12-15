using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class RaidBossDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGuildClusterID(uint guildClusterId)
        {
            return _select("GuildClusterID = @GuildClusterID", new { GuildClusterID = guildClusterId });
        }
        public IEnumerable<Schema.Record> ReadByLevel(byte level)
        {
            return _select("Level = @Level", new { Level = level });
        }
    }
}
