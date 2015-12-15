using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildActivityDailyLogSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class GuildActivityDailyLogDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByCreatedOn(uint createdOn)
        {
            return _select("CreatedOn = @CreatedOn", new { CreatedOn = createdOn });
        }
        public IEnumerable<Schema.Record> ReadByGuildIDAndMinCreatedOn(uint guildId, uint createdOn)
        {
            return _select("GuildID = @GuildID AND CreatedOn >= @CreatedOn", new { GuildID = guildId, CreatedOn = createdOn });
        }

        public Schema.Record CreateOrRead(uint guildId, uint createdOn)
        {
            var record = Create(guildId, createdOn);
            if (record != null) return record;
            return ReadByPK(new Schema.PrimaryKey { GuildID = guildId, CreatedOn = createdOn }, ReadType.Lock);
        }

        public Schema.Record Create(uint guildId, uint createdOn)
        {
            return Create(GetDefaultRecord(guildId, createdOn));
        }
        public Schema.Record GetDefaultRecord(uint guildId, uint createdOn)
        {
            return new Schema.Record
            {
                GuildID = guildId,
                BPConsuming = 0,
                DefeatRaidBossNum = 0,
                CooperationPoint = 0,
                CreatedOn = createdOn,
            };
        }
    }
}
