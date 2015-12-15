using Noroshi.Core.Game.Expedition;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerExpeditionSessionSchema;

namespace Noroshi.Server.Daos.Rdb.Expedition
{
    public class PlayerExpeditionSessionDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
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
                ExpeditionID = 0,
                ClearStep = 0,
                State = (byte)PlayerExpeditionSessionState.Inactive,
                StageData = "",
                PlayerCharacterData = "",
                StartedAt = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
