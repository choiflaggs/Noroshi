using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DailyQuestSchema;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class DailyQuestDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByTriggerID(uint triggerId)
        {
            return _select("TriggerID = @TriggerID", new { TriggerID = triggerId });
        }
    }
}
