using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class Player : AbstractController
    {
        [Post]
        public Noroshi.Core.WebApi.Response.Players.PlayerServiceResponse RecoverStamina()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerService.RecoverStamina(playerId);
        }

        [Post]
        public Noroshi.Core.WebApi.Response.Players.PlayerServiceResponse RecoverBP()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerService.RecoverBP(playerId);
        }

        [Post]
        public Core.WebApi.Response.Players.PlayerServiceResponse RecoverGold()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerService.RecoverGold(playerId);
        }


        [Post]
        public Core.WebApi.Response.Players.PlayerServiceResponse RecoverActionLevelPoint()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerService.RecoverActionLevelPoint(playerId);
        }
    }
}