namespace Noroshi.Core.WebApi.Response.Shop
{
    public class ListResponse
    {
        /// <summary>
        /// ショップ一覧。
        /// </summary>
        public Shop[] Shops { get; set; }
        /// <summary>
        /// エラー。
        /// </summary>
        public ShopError Error { get; set; }
    }
}
