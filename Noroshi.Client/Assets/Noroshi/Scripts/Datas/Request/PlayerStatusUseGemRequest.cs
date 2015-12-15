namespace Noroshi.Datas.Request
{
    public class PlayerStatusUseGemRequest
    {
        public uint Gem { get; set; }
        public ushort Count { get; set; }
        public byte Type { get; set; }
    }
}