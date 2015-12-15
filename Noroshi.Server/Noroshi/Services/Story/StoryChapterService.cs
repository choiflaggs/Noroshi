using System.Linq;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Services.Story
{
    public class StoryChapterService
    {
        public static StoryChapter[] MasterData()
        {
            return StoryChapterEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}