namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 該当プレイヤーがリーダーを務めるギルドにて、リーダーを他プレイヤーに譲った際のレスポンス。
    /// </summary>
    public class ChangeLeaderResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 新リーダー。
        /// </summary>
        public OtherPlayerStatus NewLeader { get; set; }
    }
}
