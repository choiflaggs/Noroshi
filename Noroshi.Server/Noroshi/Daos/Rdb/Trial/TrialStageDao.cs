using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrialStageSchema;

namespace Noroshi.Server.Daos.Rdb.Trial
{
    public class TrialStageDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByTrialIDs(IEnumerable<uint> trialIds)
        {
            return _select("TrialID IN @TrialIDs", new { TrialIDs = trialIds });
        }
    }
}
