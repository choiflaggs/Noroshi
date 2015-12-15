namespace Noroshi.Core.WebApi.Response
{
    public class Inventory
    {
        public PlayerGear[] Gears { get; set; }
        public PlayerGearPiece[] GearPieces { get; set; }
        public PlayerGearEnchantMaterial[] GearEnchantMaterials { get; set; }
        public PlayerSoul[] Souls { get; set; }
        public PlayerDrug[] Drugs { get; set; }
        public PlayerExchangeCashGift[] ExchangeCashGifts { get; set; }
        public PlayerRaidTicket[] RaidTickets { get; set; }
    }
}