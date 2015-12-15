using System.Collections.Generic;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGuildRaidBossSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class PlayerGuildRaidBossDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGuildRaidBossIDAndRaidBossCreatedAtOrderByDamageDesc(uint guildRaidBossId, uint guildRaidBossCreatedAt, ushort rowCount)
        {
            // パーティションを切るのでパーティションを特定するための条件を含めたクエリを発行する。
            return _select("GuildRaidBossID = @GuildRaidBossID AND GuildRaidBossCreatedAt = @GuildRaidBossCreatedAt ORDER BY Damage DESC LIMIT @RowCount", new
            {
                GuildRaidBossID = guildRaidBossId,
                GuildRaidBossCreatedAt = guildRaidBossCreatedAt,
                RowCount = rowCount,
            });
        }
        public IEnumerable<Schema.Record> ReadByPlayerIDAndStateOrderByGuildRaidBossCreatedAtAsc(uint playerId, PlayerGuildRaidBossState state, uint recordLifetime, ushort rowCount)
        {
            // パーティションを切るのでパーティションを特定するための条件を含めたクエリを発行する。
            return _select("PlayerID = @PlayerID AND State = @State AND @MinGuildRaidBossCreatedAt <= GuildRaidBossCreatedAt AND GuildRaidBossCreatedAt <= @MaxGuildRaidBossCreatedAt ORDER BY GuildRaidBossCreatedAt ASC LIMIT @RowCount", new
            {
                PlayerID = playerId,
                State = state,
                MinGuildRaidBossCreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime - recordLifetime,
                MaxGuildRaidBossCreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                RowCount = rowCount,
            });
        }

        public Schema.Record CreateOrReadAndBuild(uint playerId, uint guildRaidBossId, uint guildRaidBossCreatedAt)
        {
            var defaultRecord = new Schema.Record
            {
                PlayerID = playerId,
                GuildRaidBossID = guildRaidBossId,
                State = (byte)PlayerGuildRaidBossState.None,
                Damage = 0,
                GuildRaidBossCreatedAt = guildRaidBossCreatedAt,
            };
            return Create(defaultRecord) ?? ReadByPK(new Schema.PrimaryKey
            {
                PlayerID = playerId,
                GuildRaidBossID = guildRaidBossId,
                GuildRaidBossCreatedAt = guildRaidBossCreatedAt,
            }, ReadType.Lock);
        }
    }
}
