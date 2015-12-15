using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRequestSchema;

namespace Noroshi.Server.Entity.Guild
{
    public class GuildRequestEntity : AbstractDaoWrapperEntity<GuildRequestEntity, GuildRequestDao, Schema.PrimaryKey, Schema.Record>
    {
        public static GuildRequestEntity ReadAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { playerId }, readType).FirstOrDefault();
        }
        public static IEnumerable<GuildRequestEntity> ReadAndBuildMulti(IEnumerable<uint> playerIds, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(playerIds.Select(playerId => new Schema.PrimaryKey { PlayerID = playerId }), readType);
        }
        public static IEnumerable<GuildRequestEntity> ReadAndBuildByGuildID(uint guildId, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new GuildRequestDao()).ReadByGuildID(guildId, readType));
        }

        public static GuildRequestEntity Create(uint playerId, uint guildId)
        {
            return _instantiate((new GuildRequestDao()).Create(playerId, guildId));
        }

        public uint PlayerID => _record.PlayerID;
        public uint GuildID => _record.GuildID;

        public Core.WebApi.Response.Guild.GuildRequest ToResponseData(PlayerStatusEntity playerStatus)
        {
            if (playerStatus.PlayerID != PlayerID) throw new InvalidOperationException();
            return new Core.WebApi.Response.Guild.GuildRequest
            {
                Requester = playerStatus.ToOtherResponseData(),
            };
        }
    }
}
