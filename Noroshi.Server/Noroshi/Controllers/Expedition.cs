using LightNode.Server;
using Noroshi.Core.WebApi.Response.Expedition;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Expedition : AbstractController
    {
        [Get]
        public GetResponse Get()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ExpeditionService.Get(playerId);
        }

        [Post]
        public StartResponse Start()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ExpeditionService.Start(playerId);
        }

        [Post]
        public ReceiveRewardResponse ReceiveReward()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ExpeditionService.ReceiveReward(playerId);
        }

        [Post]
        public ResetResponse Reset()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ExpeditionService.Reset(playerId);
        }
    }
}
