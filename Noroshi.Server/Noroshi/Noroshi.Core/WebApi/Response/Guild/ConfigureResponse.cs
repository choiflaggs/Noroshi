namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 該当プレイヤーがリーダーを務めるギルドの情報を更新した際のレスポンス。
    /// </summary>
    public class ConfigureResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 更新後のギルド情報。
        /// </summary>
        public Guild Guild { get; set; }
    }
}
