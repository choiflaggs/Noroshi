using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerTrainingSchema;

namespace Noroshi.Server.Daos.Rdb.Training
{
    public class PlayerTrainingDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadOrDefaultMulti(uint playerId, IEnumerable<uint> trainingIds)
        {
            var readRecordMap = ReadMultiByPKs(trainingIds.Select(trainingId => new Schema.PrimaryKey { PlayerID = playerId, TrainingID = trainingId })).ToDictionary(r => r.TrainingID);
            return trainingIds.Select(trainingId => readRecordMap.ContainsKey(trainingId) ? readRecordMap[trainingId] : _getDefaultRecord(playerId, trainingId));
        }

        public Schema.Record CreateOrRead(uint playerId, uint trainingId)
        {
            return Create(_getDefaultRecord(playerId, trainingId)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, TrainingID = trainingId }, ReadType.Lock);
        }

        Schema.Record _getDefaultRecord(uint playerId, uint trainingId)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                TrainingID = trainingId,
                LastBattleNum = 0,
                LastBattledAt = 0,
                ReopenedAt = 0,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
