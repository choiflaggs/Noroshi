namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 該当プレイヤーがリーダーを務めるギルドにて、幹部を任命した際のレスポンス。
    /// </summary>
    public class AddExecutiveRoleResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 新幹部プレイヤー。
        /// </summary>
        public OtherPlayerStatus TargetPlayer { get; set; }
    }
}
