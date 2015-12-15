namespace Noroshi.Core.WebApi.Response.Battle
{
    public class AdditionalInformation
    {
        public string BattleTitleTextKey { get; set; }
        public Noroshi.Core.Game.Battle.WaveGaugeType? WaveGaugeType { get; set; }
        public string WaveGaugeTextKey { get; set; }
        public byte? WaveGaugeLevel { get; set; }
    }
}
