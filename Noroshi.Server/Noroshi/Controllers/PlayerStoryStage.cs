using LightNode.Server;
using Noroshi.Core.WebApi.Response.Story;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerStoryStage : AbstractController
    {
        [Post]
        public Core.WebApi.Response.Story.PlayerStoryStage Get(uint stageId) => PlayerStoryStageService.Get(stageId);

        [Get]
        public Core.WebApi.Response.Story.PlayerStoryStage[] GetAll() => PlayerStoryStageService.GetAll();

        [Post]
        public Core.WebApi.Response.Story.PlayerStoryStage[] GetByEpisodeID(uint episodeId)
            => PlayerStoryStageService.GetByEpisodeID(episodeId);

        [Post]
        public Core.WebApi.Response.Story.PlayerStoryStage UsePlayCount(uint stageId) => PlayerStoryStageService.UsePlayCount(stageId);

        [Post]
        public Core.WebApi.Response.Story.PlayerStoryStage ChangeProgress(uint stageId, byte progress)
            => PlayerStoryStageService.ChangeProgress(stageId, progress);
    }
}