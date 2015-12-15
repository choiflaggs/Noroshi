using LightNode.Server;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerStoryChapter : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Story.PlayerStoryChapter[] GetAll() => PlayerStoryChapterService.GetAll();

        [Get]
        public Core.WebApi.Response.Story.StoryChapterAndStoryEpisodeResponse[] GetAllAndMaster()
            => PlayerStoryChapterService.GetAllAndMaster();
    }
}