using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 被挨拶報酬を受け取る際のレスポンス。
    /// </summary>
    public class ReceiveGreetedRewardResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 獲得した被挨拶報酬。
        /// </summary>
        public PossessionObject[] GreetedRewards { get; set; }
    }
}
