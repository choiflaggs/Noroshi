namespace Noroshi.Core.WebApi.Response.Quest
{
    public class ListResponse
    {
        /// <summary>
        /// 閲覧可能な（デイリー）依頼リスト。
        /// </summary>
        public Quest[] Quests { get; set; }
    }
}
