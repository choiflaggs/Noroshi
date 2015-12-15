using UniRx;
using Noroshi.Core.WebApi.Response.Story;

namespace Noroshi.Story
{
    public class WebAPIRequester
    {
        public static IObservable<MasterDataResponse> MasterData()
        {
            return _getWebApiRequester().Request<MasterDataResponse>("Story/MasterData");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
