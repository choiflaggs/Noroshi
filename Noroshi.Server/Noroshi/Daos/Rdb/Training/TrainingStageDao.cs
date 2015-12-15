using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrainingStageSchema;

namespace Noroshi.Server.Daos.Rdb.Training
{
    public class TrainingStageDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByTrainingIDs(IEnumerable<uint> trainingIds)
        {
            return _select("TrainingID IN @TrainingIDs", new { TrainingIDs = trainingIds });
        }
    }
}
