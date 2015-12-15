using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryChapterSchema;

namespace Noroshi.Server.Daos.Rdb.Story
{
    public class StoryChapterDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;
    }
}