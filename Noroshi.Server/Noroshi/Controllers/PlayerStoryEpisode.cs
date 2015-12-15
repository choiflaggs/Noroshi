using LightNode.Server;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerStoryEpisode : AbstractController
    {
        [Get]
        public PlayerLastPlayStoryStageResponse GetLastStoryStage() => PlayerStoryEpisodeService.GetLastStage();

        [Post]
        public Core.WebApi.Response.Story.PlayerStoryEpisode[] ChangeLastStoryEpisode(uint episodeId)
            => PlayerStoryEpisodeService.ChangeLastEpisode(episodeId);

        [Get]
        public Core.WebApi.Response.Story.PlayerStoryEpisode[] GetAll() => PlayerStoryEpisodeService.GetAll();
    }
}