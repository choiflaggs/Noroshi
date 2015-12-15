using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerRelationSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerRelationDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultMulti(uint playerId, IEnumerable<uint> targetPlayerIds)
        {
            var readMap = ReadMultiByPKs(targetPlayerIds.Select(targetPlayerId => new Schema.PrimaryKey { PlayerID = playerId, TargetPlayerID = targetPlayerId })).ToDictionary(r => r.TargetPlayerID);
            return targetPlayerIds.Select(targetPlayerId => readMap.ContainsKey(targetPlayerId) ? readMap[targetPlayerId] : _getDefaultRecord(playerId, targetPlayerId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint targetPlayerId)
        {
            return Create(_getDefaultRecord(playerId, targetPlayerId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TargetPlayerID = targetPlayerId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId, uint targetPlayerId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TargetPlayerID = targetPlayerId,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
