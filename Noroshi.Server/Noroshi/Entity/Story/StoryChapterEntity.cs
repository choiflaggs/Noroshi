using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Story;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.StoryChapterSchema;

namespace Noroshi.Server.Entity.Story
{
    public class StoryChapterEntity : AbstractDaoWrapperEntity<StoryChapterEntity, StoryChapterDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<StoryChapterEntity> ReadAndBuildAll()
        {
            return _instantiate((new StoryChapterDao()).ReadAll());
        }

        public uint ID => _record.ID;
        public string TextKey => "Master.StoryChapter." + _record.TextKey;
        public ushort PlayerLevel => _record.PlayerLevel;
        public ushort No => _record.No;
        

        public Core.WebApi.Response.Story.StoryChapter ToResponseData()
        {
            return new Core.WebApi.Response.Story.StoryChapter
            {
                ID = ID,
                No = No,
                PlayerLevel = PlayerLevel,
                TextKey = TextKey
            };
        }
    }
}
