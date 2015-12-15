using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.QuestSchema;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class QuestDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByTriggerID(uint triggerId)
        {
            return _select("TriggerID = @TriggerID", new { TriggerID = triggerId });
        }
    }
}
