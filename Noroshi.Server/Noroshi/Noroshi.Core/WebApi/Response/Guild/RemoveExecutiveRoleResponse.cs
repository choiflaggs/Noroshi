namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 該当プレイヤーがリーダーを務めるギルドにて、幹部を解任した際のレスポンス。
    /// </summary>
    public class RemoveExecutiveRoleResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 旧幹部プレイヤー。
        /// </summary>
        public OtherPlayerStatus TargetPlayer { get; set; }
    }
}
