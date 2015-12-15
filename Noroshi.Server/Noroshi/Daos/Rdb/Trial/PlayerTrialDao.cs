using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrialSchema;

namespace Noroshi.Server.Daos.Rdb.Traial
{
    public class PlayerTrialDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultMulti(uint playerId, IEnumerable<uint> trialIds)
        {
            var readRecordMap = ReadMultiByPKs(trialIds.Select(trialId => new Schema.PrimaryKey { PlayerID = playerId, TrialID = trialId })).ToDictionary(r => r.TrialID);
            return trialIds.Select(trialId => readRecordMap.ContainsKey(trialId) ? readRecordMap[trialId] : _getDefaultRecord(playerId, trialId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint trialId)
        {
            return Create(_getDefaultRecord(playerId, trialId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TrialID = trialId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId, uint trialId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TrialID = trialId,
                ClearLevel = 0,
                LastBattleNum = 0,
                LastBattledAt = 0,
                ReopenedAt = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
