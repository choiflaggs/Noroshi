namespace Noroshi.Datas.Request
{
    public class PlayerCharacterEquipGearRequest
    {
        public uint PlayerCharacterID { get; set; }
        public uint GearID { get; set; }
        public byte Index { get; set; }
    }
}