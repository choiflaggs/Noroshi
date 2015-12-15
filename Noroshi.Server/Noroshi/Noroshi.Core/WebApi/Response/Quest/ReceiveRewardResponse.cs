using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Core.WebApi.Response.Quest
{
    public class ReceiveRewardResponse
    {
        /// <summary>
        /// 対象（デイリー）依頼。
        /// </summary>
        public Quest Quest { get; set; }
        /// <summary>
        /// 新たにオープンした（デイリー）依頼。
        /// </summary>
        public Quest NewQuest { get; set; }
        /// <summary>
        /// プレイヤー経験値獲得レスポンス。
        /// </summary>
        public AddPlayerExpResult AddPlayerExpResult { get; set; }

        /// <summary>
        /// クエストエラー通知。
        /// </summary>
        public QuestError QuestError { get; set; }
        
    }
}
