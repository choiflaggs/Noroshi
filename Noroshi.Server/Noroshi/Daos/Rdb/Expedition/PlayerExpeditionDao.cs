using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerExpeditionSchema;

namespace Noroshi.Server.Daos.Rdb.Expedition
{
    public class PlayerExpeditionDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public Schema.Record ReadOrDefault(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId }, readType) ?? _getDefaultRecord(playerId);
        }

        public Schema.Record CreateOrRead(uint playerId)
        {
            return Create(_getDefaultRecord(playerId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                ClearLevel = 0,
                LastResetNum = 0,
                LastResetedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
