using Noroshi.Server.Services.Story;

namespace Noroshi.Server.Controllers
{
    public class StoryChapter : AbstractController
    {
        public Core.WebApi.Response.Story.StoryChapter[] MasterData()
        {
            return StoryChapterService.MasterData();
        }
    }
}