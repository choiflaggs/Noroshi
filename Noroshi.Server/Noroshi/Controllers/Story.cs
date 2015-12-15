using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Story : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Story.MasterDataResponse MasterData()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return StoryService.MasterData(playerId);
        }
    }
}
