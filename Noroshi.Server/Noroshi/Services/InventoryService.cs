using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Services
{
    public class InventoryService
    {
        public static Inventory Get(uint playerId)
        {
            var gears = PlayerGearEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var gearEnchantMaterials = PlayerGearEnchantMaterialEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var gearPieces = PlayerGearPieceEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var drugs = PlayerDrugEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var raidTickets = PlayerRaidTicketEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var souls = PlayerSoulEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();
            var exchangeCashGifts =
                PlayerExchangeCashGiftEntity.ReadAndBuildAll(playerId).Select(e => e.ToResponseData()).ToArray();

            return new Inventory
            {
                Gears = gears,
                GearEnchantMaterials = gearEnchantMaterials,
                GearPieces = gearPieces,
                Drugs = drugs,
                ExchangeCashGifts = exchangeCashGifts,
                RaidTickets = raidTickets,
                Souls = souls
            };
        }
    }
}