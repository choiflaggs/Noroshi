using UniRx;
using Noroshi.Core.WebApi.Request.Gacha;
using Noroshi.Core.WebApi.Response.Gacha;

namespace Noroshi.Gacha
{
    public class WebApiRequester
    {
        /// <summary>
        /// ガチャエントリーポイント一覧を取得する。
        /// </summary>
        public static IObservable<EntryPointListResponse> EntryPointList()
        {
            return _getWebApiRequester().Request<EntryPointListResponse>("Gacha/EntryPointList");
        }
        /// <summary>
        /// ガチャる。
        /// </summary>
        /// <param name="gachaEntryPointID">ガチャエントリーポイントID</param>
        /// <param name="free">無料試行</param>
        public static IObservable<LotResponse> Lot(uint gachaEntryPointID, bool free = false)
        {
            var requestParam = new LotRequest
            {
                GachaEntryPointID = gachaEntryPointID,
                Free = free,
            };
            return _getWebApiRequester().Post<LotRequest, LotResponse>("Gacha/Lot", requestParam);
        }
        /// <summary>
        /// チュートリアルガチャをガチャる。
        /// </summary>
        public static IObservable<LotResponse> LotTutorialGacha()
        {
            return _getWebApiRequester().Post<LotResponse>("Gacha/LotTutorialGacha");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}