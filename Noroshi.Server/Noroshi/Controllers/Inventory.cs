using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Inventory : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Inventory Get()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return InventoryService.Get(playerId);
        }
    }
}