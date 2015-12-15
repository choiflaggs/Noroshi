namespace Noroshi.Datas.Request
{
    public class PlayerItemUseWithCharacterRequest
    {
        public uint ItemID { get; set; }
        public uint CharacterID { get; set; }
        public uint UsePossessionsCount { get; set; }
    }
}