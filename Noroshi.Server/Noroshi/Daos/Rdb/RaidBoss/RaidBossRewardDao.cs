using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossRewardSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class RaidBossRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByRaidBossIDs(IEnumerable<uint> raidBossIds)
        {
            return _select("RaidBossID IN @RaidBossIDs", new { RaidBossIDs = raidBossIds });
        }
    }
}
