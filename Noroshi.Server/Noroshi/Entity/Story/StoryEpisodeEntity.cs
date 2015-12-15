using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Story;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryEpisodeSchema;

namespace Noroshi.Server.Entity.Story
{
    public class StoryEpisodeEntity : AbstractDaoWrapperEntity<StoryEpisodeEntity, StoryEpisodeDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<StoryEpisodeEntity> ReadAndBuildAll()
        {
            return _instantiate((new StoryEpisodeDao()).ReadAll());
        }

        public static IEnumerable<StoryEpisodeEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }));
        }


        public uint ID => _record.ID;
        public uint ChapterID => _record.ChapterID;
        public string TextKey => "Master.StoryEpisode." + _record.TextKey;
        public ushort No => _record.No;

        public Core.WebApi.Response.Story.StoryEpisode ToResponseData()
        {
            return new Core.WebApi.Response.Story.StoryEpisode
            {
                ID = ID,
                No = No,
                ChapterID = ChapterID,
                TextKey = TextKey
            };
        }
    }
}
