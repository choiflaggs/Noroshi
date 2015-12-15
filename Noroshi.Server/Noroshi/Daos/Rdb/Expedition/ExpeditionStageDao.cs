using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExpeditionStageSchema;

namespace Noroshi.Server.Daos.Rdb.Expedition
{
    public class ExpeditionStageDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByExpeditionIDs(IEnumerable<uint> expeditionIds)
        {
            return _select("ExpeditionID IN @ExpeditionIDs", new { ExpeditionIDs = expeditionIds });
        }
    }
}
