namespace Noroshi.Core.WebApi.Response.Guild
{
    public class GetResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// ギルド情報。
        /// </summary>
        public Guild Guild { get; set; }
        /// <summary>
        /// リーダー情報。
        /// </summary>
        public OtherPlayerStatus Leader { get; set; }
    }
}
