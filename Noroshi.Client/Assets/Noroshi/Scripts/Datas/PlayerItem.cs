namespace Noroshi.Datas
{
    public class PlayerItem : PlayerData, IPersonalItem
    {
        public uint ItemID { get; set; }
        public uint PlayerID { get; set; }
        public uint PossessionsCount { get; set; }
    }
}