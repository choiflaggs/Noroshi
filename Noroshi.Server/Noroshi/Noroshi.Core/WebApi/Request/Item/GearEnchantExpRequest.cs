namespace Noroshi.Core.WebApi.Request.Item
{
    class GearEnchantExpRequest
    {
        public uint PlayerGearID { get; set; }
        public uint[] GearIDs { get; set; }
        public uint[] GearEnchantMaterialIDs { get; set; }
        public uint[] GearPieceIDs { get; set; }
    }
}
