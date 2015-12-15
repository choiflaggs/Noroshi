using Noroshi.Core.Game.Battle;

namespace Noroshi.Server.Entity.Battle
{
    public class AdditionalInformation
    {
        public string BattleTitleTextKey { get; set; }
        public WaveGaugeType? WaveGaugeType { get; set; }
        public string WaveGaugeTextKey { get; set; }
        public byte? WaveGaugeLevel { get; set; }

        public Core.WebApi.Response.Battle.AdditionalInformation ToResponseData()
        {
            return new Core.WebApi.Response.Battle.AdditionalInformation
            {
                BattleTitleTextKey = BattleTitleTextKey,
                WaveGaugeType = WaveGaugeType,
                WaveGaugeTextKey = WaveGaugeTextKey,
                WaveGaugeLevel = WaveGaugeLevel,
            };
        }
    }
}
