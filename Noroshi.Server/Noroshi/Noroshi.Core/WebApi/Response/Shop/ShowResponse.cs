namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// 指定プレイヤーの指定ショップ内容を取得した際のレスポンス。
    /// </summary>
    public class ShowResponse
    {
        /// <summary>
        /// ショップ。
        /// </summary>
        public Shop Shop { get; set; }
        /// <summary>
        /// エラー。
        /// </summary>
        public ShopError Error { get; set; }
    }
}
