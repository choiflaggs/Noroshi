using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrainingStageSchema;

namespace Noroshi.Server.Daos.Rdb.Training
{
    public class PlayerTrainingStageDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultMulti(uint playerId, IEnumerable<uint> trainingStageIds)
        {
            var readRecordMap = ReadMultiByPKs(trainingStageIds.Select(trainingStageId => new Schema.PrimaryKey { PlayerID = playerId, TrainingStageID = trainingStageId })).ToDictionary(r => r.TrainingStageID);
            return trainingStageIds.Select(trainingStageId => readRecordMap.ContainsKey(trainingStageId) ? readRecordMap[trainingStageId] : _getDefaultRecord(playerId, trainingStageId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint trainingStageId)
        {
            return Create(_getDefaultRecord(playerId, trainingStageId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TrainingStageID = trainingStageId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId, uint trainingStageId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TrainingStageID = trainingStageId,
                Score = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
