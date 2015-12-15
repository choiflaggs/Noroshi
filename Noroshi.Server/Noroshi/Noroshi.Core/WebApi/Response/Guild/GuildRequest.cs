namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 加入リクエスト。
    /// </summary>
    public class GuildRequest
    {
        /// <summary>
        /// 加入申請プレイヤー。
        /// </summary>
        public OtherPlayerStatus Requester { get; set; }
    }
}
