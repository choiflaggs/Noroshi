using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.QuestRewardSchema;

namespace Noroshi.Server.Daos.Rdb.Quest
{
    public class QuestRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByQuestIDs(IEnumerable<uint> questIds)
        {
            return _select("QuestID IN @QuestIDs", new { QuestIDs = questIds });
        }
    }
}
