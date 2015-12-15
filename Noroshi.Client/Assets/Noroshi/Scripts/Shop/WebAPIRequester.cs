using UniRx;
using Noroshi.Core.WebApi.Request.Shop;
using Noroshi.Core.WebApi.Response.Shop;

namespace Noroshi.Shop
{
    public class WebAPIRequester
    {
        /// <summary>
        /// ショップ一覧を取得する。
        /// </summary>
        public static IObservable<ListResponse> List()
        {
            return _getWebApiRequester().Request<ListResponse>("Shop/List");
        }
        /// <summary>
        /// 商品を購入する。
        /// </summary>
        /// <param name="shopDisplayId">ショップディスプレイ ID</param>
        public static IObservable<BuyResponse> Buy(uint shopDisplayId)
        {
            var requestParam = new BuyRequest
            {
                ShopDisplayID = shopDisplayId,
            };
            return _getWebApiRequester().Post<BuyRequest, BuyResponse>("Shop/Buy", requestParam);
        }
        /// <summary>
        /// 商品を更新する。
        /// </summary>
        /// <param name="shopId">ショップ ID</param>
        public static IObservable<ShowResponse> UpdateMerchandises(uint shopId)
        {
            var requestParam = new ShopRequest
            {
                ShopID = shopId,
            };
            return _getWebApiRequester().Post<ShopRequest, ShowResponse>("Shop/UpdateMerchandises", requestParam);
        }
        /// <summary>
        /// ショップアクセス時の自動売却を実行する。
        /// </summary>
        public static IObservable<SellAutomaticallyResponse> SellAutomatically()
        {
            return _getWebApiRequester().Post<SellAutomaticallyResponse>("Shop/SellAutomatically");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}