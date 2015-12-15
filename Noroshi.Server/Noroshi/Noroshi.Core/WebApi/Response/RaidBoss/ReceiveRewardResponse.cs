using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.RaidBoss
{
    public class ReceiveRewardResponse
    {
        public RaidBossError Error { get; set; }
        public PossessionObject[] DiscoveryRewards { get; set; }
        public PossessionObject[] EntryRewards { get; set; }
    }
}
