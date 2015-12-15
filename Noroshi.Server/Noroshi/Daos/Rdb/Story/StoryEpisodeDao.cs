using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryEpisodeSchema;

namespace Noroshi.Server.Daos.Rdb.Story
{
    public class StoryEpisodeDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByChapterID(uint chapterId)
        {
            return _select("ChapterID = @ChapterID", new { ChapterID = chapterId });
        }
    }
}