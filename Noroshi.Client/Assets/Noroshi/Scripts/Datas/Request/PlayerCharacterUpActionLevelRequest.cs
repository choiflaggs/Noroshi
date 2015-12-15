namespace Noroshi.Datas.Request
{
    public class PlayerCharacterUpActionLevelRequest
    {
        public uint PlayerCharacterID { get; set; }
        public ushort Level { get; set; }
        public uint Index { get; set; }
    }
}