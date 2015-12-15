namespace Noroshi.Core.WebApi.Response.Shop
{
    /// <summary>
    /// ショップ関連のエラーレスポンスを扱う。
    /// </summary>
    public class ShopError
    {
        /// <summary>
        /// 対象のショップがオープンしていないエラー。
        /// </summary>
        public bool ShopIsNotOpen { get; set; }
        /// <summary>
        /// 商品入れ替えをすべきエラー。ショップを再ロードする必要あり。
        /// </summary>
        public bool ShouldUpdateMerchandises { get; set; }
        /// <summary>
        /// 売れるものがありません。
        /// </summary>
        public bool NoSellableItem { get; set; }
    }
}
