namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// 指定プレイヤーに指定商品を購入させた際のレスポンス。
    /// </summary>
    public class BuyResponse
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
