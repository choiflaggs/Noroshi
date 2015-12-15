using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerActivityDailyLogSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerActivityDailyLogDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerIDsAndMinCreatedOn(IEnumerable<uint> playerIds, uint createdOn)
        {
            return _select("PlayerID IN @PlayerIDs AND CreatedOn >= @CreatedOn", new { PlayerIDs = playerIds, CreatedOn = createdOn });
        }

        public Schema.Record CreateOrRead(uint playerId, uint createdOn)
        {
            var record = Create(playerId, createdOn);
            if (record != null) return record;
            return ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, CreatedOn = createdOn }, ReadType.Lock);
        }

        public Schema.Record Create(uint playerId, uint createdOn)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                StaminaConsuming = 0,
                BPConsuming = 0,
                CreatedOn = createdOn,
            };
            return Create(record);
        }
    }
}
