using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Item;
using GearEnchantMaterial = Noroshi.Core.WebApi.Response.GearEnchantMaterial;

namespace Noroshi.Server.Services.Item
{
    public class GearEnchantMaterialService
    {
        public static PlayerGearEnchantMaterial[] GetAll(uint playerId)
        {
            return PlayerGearEnchantMaterialEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static GearEnchantMaterial[] MasterData()
        {
            return GearEnchantMaterialEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }
    }
}