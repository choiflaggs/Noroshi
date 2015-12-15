using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRequestSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class GuildRequestDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;


        public IEnumerable<Schema.Record> ReadByGuildID(uint guildId, ReadType readType = ReadType.Slave)
        {
            return _select("GuildID = @GuildID ORDER BY CreatedAt Desc", new { GuildID = guildId }, readType);
        }

        public Schema.Record Create(uint playerId, uint guildId)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                GuildID = guildId,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }
    }
}
