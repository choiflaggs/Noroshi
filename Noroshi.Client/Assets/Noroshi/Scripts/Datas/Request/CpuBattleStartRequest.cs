namespace Noroshi.Datas.Request
{
    public class CpuBattleStartRequest
    {
        public byte Category { get; set; }
        public uint ID { get; set; }
        public uint[] PlayerCharacterIDs { get; set; }
        public uint? RentalPlayerCharacterID { get; set; }
        public uint PaymentNum { get; set; }
    }
}
