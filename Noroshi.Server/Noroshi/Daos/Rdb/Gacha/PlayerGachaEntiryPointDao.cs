using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGachaEntryPointSchema;

namespace Noroshi.Server.Daos.Rdb.Gacha
{
    public class PlayerGachaEntiryPointDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultByPlayerIDAndGachaEntryPointIDs(uint playerId, IEnumerable<uint> gachaEntryPointIds)
        {
            var map = _select("PlayerID = @PlayerID AND GachaEntryPointID IN @GachaEntryPointIDs", new { PlayerID = playerId, GachaEntryPointIDs = gachaEntryPointIds })
                .ToDictionary(r => r.GachaEntryPointID);
            return gachaEntryPointIds.Select(gachaEntryPointId => map.ContainsKey(gachaEntryPointId) ? map[gachaEntryPointId] : GetDefaultRecord(playerId, gachaEntryPointId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint gachaEntryPointId)
        {
            var record = Create(GetDefaultRecord(playerId, gachaEntryPointId));
            return record ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, GachaEntryPointID = gachaEntryPointId }, ReadType.Lock);
        }

        public Schema.Record GetDefaultRecord(uint playerId, uint gachaEntryPointId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                GachaEntryPointID = gachaEntryPointId,
                TotalLotNum = 0,
                FreeReopenedAt = 0,
                LastFreeLotNum = 0,
                LastFreeLottedAt = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
