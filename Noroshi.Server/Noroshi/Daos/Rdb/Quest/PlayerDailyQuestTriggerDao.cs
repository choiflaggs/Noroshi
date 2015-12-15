using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerDailyQuestTriggerSchema;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class PlayerDailyQuestTriggerDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID", new { PlayerID = playerId });
        }

        public Schema.Record Create(uint playerId, uint triggerId, uint currentNum)
        {
            return Create( GetDefaultRecord(playerId, triggerId, currentNum) );
        }

        public Schema.Record CreateOrRead(uint playerId, uint triggerId, uint currentNum)
        {
            var record = Create(playerId, triggerId, currentNum);
            if (record != null) return record;
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TriggerID = triggerId }, ReadType.Lock);
        }

        public Schema.Record GetDefaultRecord(uint playerId, uint triggerId, uint currentNum)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TriggerID = triggerId,
                CurrentNum = currentNum,
                ReceiveRewardThreshold = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }


    }
}
