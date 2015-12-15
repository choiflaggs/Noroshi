namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// 指定プレイヤーに指定ショップの商品を更新させた際のレスポンス。
    /// </summary>
    public class UpdateMerchandisesResponse
    {
        /// <summary>
        /// 更新後のショップ。
        /// </summary>
        public Shop Shop { get; set; }
        /// <summary>
        /// エラー。
        /// </summary>
        public ShopError Error { get; set; }
    }
}
