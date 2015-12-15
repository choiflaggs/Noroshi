using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrialStageSchema;

namespace Noroshi.Server.Daos.Rdb.Trial
{
    public class PlayerTrialStageDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultMulti(uint playerId, IEnumerable<uint> trialStageIds)
        {
            var readRecordMap = ReadMultiByPKs(trialStageIds.Select(trialStageId => new Schema.PrimaryKey { PlayerID = playerId, TrialStageID = trialStageId })).ToDictionary(r => r.TrialStageID);
            return trialStageIds.Select(trialStageId => readRecordMap.ContainsKey(trialStageId) ? readRecordMap[trialStageId] : _getDefaultRecord(playerId, trialStageId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint trialStageId)
        {
            return Create(_getDefaultRecord(playerId, trialStageId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TrialStageID = trialStageId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId, uint trialStageId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TrialStageID = trialStageId,
                Rank = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
