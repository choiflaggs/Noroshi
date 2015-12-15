using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Services.Item
{
    public class GearService
    {
        public static PlayerGear[] GetAll(uint playerId)
        {
            return PlayerGearEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static Gear[] MasterData()
        {
            var gearEnchantLevelData = GearEnchantLevelEntity.ReadAndBuildAll();
            var gearEnchantExpData = GearEnchantExpEntity.ReadAndBuildAll();
            return GearEntity.ReadAndBuildAll().Select(data =>
            {
                var responseData = data.ToResponseData();
                responseData.GearEnchantExp =
                    gearEnchantExpData.First(enchantExp => responseData.ID == enchantExp.GearID).ToResponseData();
                responseData.GearEnchantLevels =
                    gearEnchantLevelData.Where(enchantLevel => enchantLevel.GearID == responseData.ID)
                        .Select(e => e.ToResponseData()).ToArray();
                return responseData;

            }).ToArray();
        }
    }
}