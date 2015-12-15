using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Noroshi.Core.WebApi.Response.Guild;

using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildChatMessageSchema;
namespace Noroshi.Server.Entity.Guild
{
    public class GuildChatMessageEntity : AbstractDaoWrapperEntity<GuildChatMessageEntity, GuildChatMessageDao, Schema.PrimaryKey, Schema.Record>
    {

        public static GuildChatMessageEntity Create(uint guildId, uint playerId, string message)
        {
            return _instantiate((new GuildChatMessageDao()).Create(guildId, playerId, message));
        }

        public static IEnumerable<GuildChatMessageEntity> ReadByGuildID(uint guildId)
        {
            return _instantiate((new GuildChatMessageDao()).ReadByGuildID(guildId, Constant.GUILD_CHAT_ROW_LIMIT));
        }
        
        public static IEnumerable<GuildChatMessageEntity> ReadNewByGuildIDAndCurrentCreatedAtAndCurrentID(uint guildId, uint currentCreatedAt, uint currentMessageId)
        {
            return _instantiate((new GuildChatMessageDao()).ReadNewByGuildIDAndCurrentCreatedAtAndCurrentID(guildId, currentCreatedAt, currentMessageId, Constant.GUILD_CHAT_ROW_LIMIT));
        }

        public static IEnumerable<GuildChatMessageEntity> ReadNewByGuildIDAndCurrentCreatedAt(uint guildId, uint currentCreatedAt, ushort rowCount)
        {
            return _instantiate((new GuildChatMessageDao()).ReadNewByGuildIDAndCurrentCreatedAt(guildId, currentCreatedAt, rowCount));
        }

        public static IEnumerable<GuildChatMessageEntity> ReadOldByGuildIDAndCurrentCreatedAtAndCurrentID(uint guildId, uint currentCreatedAt, uint currentMessageId)
        {
            return _instantiate((new GuildChatMessageDao()).ReadOldByGuildIDAndCurrentCreatedAtAndCurrentID(guildId, currentCreatedAt, currentMessageId, Constant.GUILD_CHAT_ROW_LIMIT));
        }

        public static IEnumerable<GuildChatMessageEntity> ReadOldByGuildIDAndCurrentCreatedAt(uint guildId, uint currentCreatedAt, ushort rowCount)
        {
            return _instantiate((new GuildChatMessageDao()).ReadOldByGuildIDAndCurrentCreatedAt(guildId, currentCreatedAt, rowCount));
        }

        public uint ID => _record.ID;
        public uint PlayerID => _record.PlayerID;
        public uint GuildID => _record.GuildID;
        public string Message => _record.Message;

        public GuildChatMessage ToResponseData(PlayerStatusEntity playerStatus)
        {
            return new GuildChatMessage
            {
                OtherPlayerStatus = playerStatus == null ? null : playerStatus.ToOtherResponseData(),
                ID          = _record.ID,
                Message     = _record.Message,
                CreatedAt   = _record.CreatedAt
            };
        }
    }

}