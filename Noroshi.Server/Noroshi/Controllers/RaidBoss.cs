using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class RaidBoss : AbstractController
    {
        [Get]
        public Core.WebApi.Response.RaidBoss.ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RaidBossService.List(playerId);
        }
        [Get]
        public Core.WebApi.Response.RaidBoss.ShowResponse Show(uint guildRaidBossId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RaidBossService.Show(playerId, guildRaidBossId);
        }
        [Post]
        public Core.WebApi.Response.RaidBoss.ReceiveRewardResponse ReceiveReward(uint guildRaidBossId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RaidBossService.ReceiveReward(playerId, guildRaidBossId);
        }
    }
}
