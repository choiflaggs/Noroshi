using System.Collections.Generic;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRaidBossSchema;

namespace Noroshi.Server.Daos.Rdb.RaidBoss
{
    public class GuildRaidBossDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByGuildIDAndStateWithLifetime(uint guildId, RaidBossState state, uint recordLifetime, ReadType readType)
        {
            return _select("GuildID = @GuildID AND State = @State AND @MinCreatedAt <= CreatedAt", new
            {
                GuildID = guildId,
                State = state,
                MinCreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime - recordLifetime,
            }, readType);
        }

        public Schema.Record Create(uint playerId, uint guildId, uint raidBossId)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                GuildID = guildId,
                RaidBossID = raidBossId,
                DiscoveryPlayerID = playerId,
                BattleData = "",
                EntryData = "",
            };
            return Create(record);
        }
    }
}
