using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Quest : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Quest.ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return QuestService.List(playerId);
        }
        [Post]
        public Core.WebApi.Response.Quest.ReceiveRewardResponse ReceiveReward(uint questId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return QuestService.ReceiveReward(playerId, questId);
        }

        [Get]
        public Core.WebApi.Response.Quest.ListResponse DailyList()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return QuestService.DailyList(playerId);
        }
        [Post]
        public Core.WebApi.Response.Quest.ReceiveRewardResponse ReceiveDailyReward(uint dailyQuestId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return QuestService.ReceiveDailyReward(playerId, dailyQuestId);
        }
    }
}
