using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryStageSchema;

namespace Noroshi.Server.Daos.Rdb.Story
{
    public class StoryStageDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByEpisodeID(uint episodeId)
        {
            return _select("EpisodeID = @EpisodeID", new { EpisodeID = episodeId });
        }
    }
}