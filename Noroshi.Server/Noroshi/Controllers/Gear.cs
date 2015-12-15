using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class Gear : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Gear[] MasterData() => GearService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerGear[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GearService.GetAll(playerId);
        }

        [Post]
        public Core.WebApi.Response.GearCraft GearCraft(uint gearId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return CraftService.GearCraft(playerId, gearId);
        }

        [Post]
        public Core.WebApi.Response.GearEnchantAddExpResponse GearEnchantExp(uint playerGearId, uint[] gearIds, uint[] gearEnchantMaterialIds, uint[] gearPieceIds)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GearEnchantService.AddExp(playerId, playerGearId, gearIds, gearEnchantMaterialIds, gearPieceIds);
        }
    }
}