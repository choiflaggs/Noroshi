namespace Noroshi.Core.WebApi.Response
{
    public class GearEnchantLevel
    {
        public uint GearID { get; set; }
        public byte EnchantLevel { get; set; }
        public float GrowthRate { get; set; }
        public uint Exp { get; set; }
        public uint Gold { get; set; }
    }
}