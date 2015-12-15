using System.Collections.Generic;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildChatMessageSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class GuildChatMessageDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record Create(uint guildId, uint playerId, string message)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                GuildID   = guildId,
                PlayerID  = playerId,
                Status    = (byte) GuildMessageCategory.Normal,
                Message   = message,
            };
            return Create(record);
        }

        public IEnumerable<Schema.Record> ReadByGuildID(uint guildId, ushort rowCount)
        {
            return _select(
                "GuildID = @GuildID AND Status = @Status AND CreatedAt BETWEEN @LimitCreatedAt AND @CreatedAt ORDER BY CreatedAt DESC, ID DESC LIMIT @RowCount",
                new
                {
                    GuildID             = guildId,
                    Status              = (byte) GuildMessageCategory.Normal,
                    CreatedAt           = ContextContainer.GetContext().TimeHandler.UnixTime,
                    LimitCreatedAt      = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - Constant.BEGINNER_GUILD_CHAT_DROPOUT_AFTER_SPAN.TotalSeconds,
                    RowCount            = rowCount
                }
            );
        }

        public IEnumerable<Schema.Record> ReadNewByGuildIDAndCurrentCreatedAtAndCurrentID(uint guildId, uint currentCreatedAt, uint CurrentId,ushort rowCount)
        {
            return _select(
                "GuildID = @GuildID AND Status = @Status AND CreatedAt = @CreatedAt AND ID > @ID ORDER BY ID ASC LIMIT @RowCount",
                new
                {
                    GuildID             = guildId,
                    Status              = (byte) GuildMessageCategory.Normal,
                    CreatedAt           = currentCreatedAt,
                    ID                  = CurrentId,
                    RowCount            = rowCount
                }
            );
        }

        public IEnumerable<Schema.Record> ReadNewByGuildIDAndCurrentCreatedAt(uint guildId, uint currentCreatedAt, ushort rowCount)
        {
            return _select(
                "GuildID = @GuildID AND Status = @Status AND CreatedAt > @CreatedAt AND CreatedAt < @LimitCreatedAt ORDER BY CreatedAt ASC, ID ASC LIMIT @RowCount",
                new
                {
                    GuildID            = guildId,
                    Status             = (byte) GuildMessageCategory.Normal,
                    CreatedAt          = currentCreatedAt,
                    LimitCreatedAt     = ContextContainer.GetContext().TimeHandler.UnixTime,
                    RowCount           = rowCount
                }

            );
        }

        public IEnumerable<Schema.Record> ReadOldByGuildIDAndCurrentCreatedAtAndCurrentID(uint guildId, uint currentCreatedAt, uint CurrentId, ushort rowCount)
        {
            return _select(
                "GuildID = @GuildID AND Status = @Status AND CreatedAt = @CreatedAt AND ID < @ID ORDER BY ID DESC LIMIT @RowCount",
                new
                {
                    GuildID            = guildId,
                    Status             = (byte) GuildMessageCategory.Normal,
                    CreatedAt          = currentCreatedAt,
                    ID                 = CurrentId,
                    RowCount           = rowCount
                }
            );
        }

        public IEnumerable<Schema.Record> ReadOldByGuildIDAndCurrentCreatedAt(uint guildId, uint currentCreatedAt, ushort rowCount)
        {
            return _select(
                "GuildID = @GuildID AND Status = @Status AND CreatedAt < @CreatedAt AND CreatedAt > @LimitCreatedAt ORDER BY CreatedAt DESC, ID DESC LIMIT @RowCount",
                new
                {
                    GuildID         = guildId,
                    Status          = (byte) GuildMessageCategory.Normal,
                    CreatedAt       = currentCreatedAt,
                    LimitCreatedAt  = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - Constant.BEGINNER_GUILD_CHAT_DROPOUT_AFTER_SPAN.TotalSeconds,
                    RowCount        = rowCount
                }
            );
        }

    }
}
