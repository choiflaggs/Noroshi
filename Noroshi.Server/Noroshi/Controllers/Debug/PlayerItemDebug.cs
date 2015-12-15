using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Debug;

namespace Noroshi.Server.Controllers.Debug
{
    public class PlayerItemDebug : AbstractController
    {
        [Post]
        public Core.WebApi.Response.PlayerItem GetItem(uint itemId, ushort addPossessionsCount)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerItemDebugService.GetItem(playerId, itemId, addPossessionsCount);
        }

        [Post]
        public Core.WebApi.Response.PlayerItem UseItem(uint itemId, ushort usePossessionsCount)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerItemDebugService.UseItem(playerId, itemId, usePossessionsCount);
        }
    }
}