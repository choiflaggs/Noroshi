using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class GearEnchantMaterial : AbstractController
    {
        [Get]
        public Core.WebApi.Response.GearEnchantMaterial[] MasterData() => GearEnchantMaterialService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerGearEnchantMaterial[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GearEnchantMaterialService.GetAll(playerId);
        }
    }
}