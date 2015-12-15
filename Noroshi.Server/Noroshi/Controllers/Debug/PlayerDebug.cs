using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Debug;

namespace Noroshi.Server.Controllers.Debug
{
    public class PlayerDebug : AbstractController
    {
        [Post]
        public Core.WebApi.Response.Debug.PlayerDebugResponse Reset()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerDebugService.Reset(playerId);
        }
        [Post]
        public Core.WebApi.Response.Debug.PlayerDebugResponse Swap(uint targetPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerDebugService.Swap(playerId, targetPlayerId);
        }
    }
}
