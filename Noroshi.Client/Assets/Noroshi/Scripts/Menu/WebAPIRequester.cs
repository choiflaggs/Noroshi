using UniRx;
using Noroshi.Core.WebApi.Request.Quest;
using Noroshi.Core.WebApi.Response.Quest;

namespace Noroshi.Menu
{
    public class WebApiRequester
    {
        public static IObservable<ListResponse> DailyList()
        {
            return _getWebApiRequester().Request<ListResponse>("Quest/DailyList");
        }

        public static IObservable<ListResponse> QuestList()
        {
            return _getWebApiRequester().Request<ListResponse>("Quest/List");
        }
        
        public static IObservable<ReceiveRewardResponse> ReceiveDailyReward(uint dailyQuestID)
        {
            var requestParams = new ReceiveDailyRewardRequest
            {
                DailyQuestID = dailyQuestID,
            };
            return _getWebApiRequester().Post<ReceiveDailyRewardRequest, ReceiveRewardResponse>("Quest/ReceiveDailyReward", requestParams);
        }

        public static IObservable<ReceiveRewardResponse> ReceiveReward(uint questID)
        {
            var requestParams = new ReceiveRewardRequest
            {
                QuestID = questID,
            };
            return _getWebApiRequester().Post<ReceiveRewardRequest, ReceiveRewardResponse>("Quest/ReceiveReward", requestParams);
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
