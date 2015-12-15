using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleStoryMessageSchema;

namespace Noroshi.Server.Daos.Rdb.Battle
{
    public class CpuBattleStoryMessageDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByStoryIDs(IEnumerable<uint> storyIds)
        {
            return _select("StoryID IN @StoryIDs", new { StoryIDs = storyIds });
        }
    }
}
