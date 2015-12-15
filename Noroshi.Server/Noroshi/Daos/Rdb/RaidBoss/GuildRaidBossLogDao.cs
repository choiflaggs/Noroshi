using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRaidBossLogSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class GuildRaidBossLogDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGuildRaidBossIDOrderByCreatedAtDesc(uint guildRaidBossId, uint guildRaidBossCreatedAt, ushort rowCount)
        {
            // パーティションを切るのでパーティションを特定するための条件を含めたクエリを発行する。
            return _select("GuildRaidBossID = @GuildRaidBossID AND GuildRaidBossCreatedAt = @GuildRaidBossCreatedAt ORDER BY CreatedAt DESC LIMIT @RowCount", new
            {
                GuildRaidBossID = guildRaidBossId,
                GuildRaidBossCreatedAt = guildRaidBossCreatedAt,
                RowCount = rowCount,
            });
        }

        public Schema.Record Create(uint guildRaidBossId, uint guildRaidBossCreatedAt, uint playerId, uint damage)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                GuildRaidBossID = guildRaidBossId,
                PlayerID = playerId,
                Damage = damage,
                GuildRaidBossCreatedAt = guildRaidBossCreatedAt,
            };
            return Create(record);
        }
    }
}
