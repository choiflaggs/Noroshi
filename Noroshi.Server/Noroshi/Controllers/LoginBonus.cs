using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class LoginBonus : AbstractController
    {
        [Get]
        public Core.WebApi.Response.LoginBonus.ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return LoginBonusService.List(playerId);
        }
        [Post]
        public Core.WebApi.Response.LoginBonus.ReceiveRewardResponse ReceiveReward(uint loginBonusId, byte threshold)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return LoginBonusService.ReceiveReward(playerId, loginBonusId, threshold);
        }
    }
}
