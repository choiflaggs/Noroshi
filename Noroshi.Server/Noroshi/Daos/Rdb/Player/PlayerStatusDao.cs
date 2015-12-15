using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStatusSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerStatusDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;


        public IEnumerable<Schema.Record> ReadByGuildID(uint guildId, byte rowCount, ReadType readType = ReadType.Slave)
        {
            return _select("GuildID = @GuildID ORDER BY UpdatedAt DESC LIMIT @RowCount", new { GuildID = guildId, RowCount = rowCount }, readType);
        }

        public IEnumerable<Schema.Record> ReadByExpRange(uint minExp, uint maxExp, byte rowCount)
        {
            return _select("@MinExp <= Exp AND Exp <= @MaxExp ORDER BY Exp DESC Limit @RowCount", new { MinExp = minExp, MaxExp = maxExp, RowCount = rowCount });
        }

        public Schema.Record Create(uint playerId, uint languageId, uint shardId)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                LanguageID = languageId,
                Name = $"Player{playerId}",
                Exp = 0,
                FreeGem = 0,
                ChargeGem = 0,
                GuildID = 0,
                Gold = 0,
                AvaterCharacterID = 0,
                VipExp = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,

                GuildRole              = 0,
                GuildState             = 0,
                LastGreetingNum        = 0,
                LastGreetedAt          = ContextContainer.GetContext().TimeHandler.UnixTime,
                UnconfirmedGreetedNum  = 0,

                LastBP                 = 0,
                LastBPUpdatedAt        = 0,
                LastBPRecoveryNum      = 0,
                LastBPRecoveredAt      = ContextContainer.GetContext().TimeHandler.UnixTime,

                LastStamina            = 0,
                LastStaminaUpdatedAt   = 0,
                LastStaminaRecoveryNum = 0,
                LastStaminaRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime,

                LastActionLevelPoint = 0,
                LastActionLevelPointUpdatedAt = 0,
                LastActionLevelPointRecoveryNum = 0,
                LastActionLevelPointRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime,

                LastGoldRecoveryNum = 0,
                LastGoldRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime,

                TutorialStep           = 0
            };
            return Create(record, shardId) ? record : null;
        }
    }
}
