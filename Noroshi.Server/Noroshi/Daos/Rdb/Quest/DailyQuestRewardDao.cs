using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DailyQuestRewardSchema;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class DailyQuestRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByDailyQuestIDs(IEnumerable<uint> dailyQuestIds)
        {
            return _select("DailyQuestID IN @DailyQuestIDs", new { DailyQuestIDs = dailyQuestIds });
        }
    }
}
