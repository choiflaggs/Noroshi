namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 該当プレイヤーがリーダーを務めるギルドにて、所属プレイヤーを除名した際のレスポンス。
    /// </summary>
    public class LayOffResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 除名プレイヤー。
        /// </summary>
        public OtherPlayerStatus TargetPlayer { get; set; }
    }
}
