namespace Noroshi.Core.WebApi.Response
{
    public class PlayerCharacterGear
    {
        public uint ID { get; set; }
        public uint GearID { get; set; }
        public uint Exp { get; set; }
        public ushort Level { get; set; }
        public byte GearPosition { get; set; }

    }
}