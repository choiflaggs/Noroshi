namespace Noroshi.Core.WebApi.Response.Information
{
    /// <summary>
    /// 指定プレイヤーのお知らせ一覧取得した際のレスポンス。
    /// </summary>
    public class ListResponse
    {
        /// <summary>
        /// お知らせ一覧。
        /// </summary>
        public Information[] Informations { get; set; }
    }
}
