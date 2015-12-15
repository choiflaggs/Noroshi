using UniRx;
using Noroshi.Core.WebApi.Response.Expedition;

namespace Noroshi.Expedition
{
    public class WebApiRequester
    {
        public static IObservable<GetResponse> Get()
        {
            return _getWebApiRequester().Request<GetResponse>("Expedition/Get");
        }

        public static IObservable<StartResponse> Start()
        {
            return _getWebApiRequester().Post<StartResponse>("Expedition/Start");
        }

        public static IObservable<ReceiveRewardResponse> ReceiveReward()
        {
            return _getWebApiRequester().Post<ReceiveRewardResponse>("Expedition/ReceiveReward");
        }

        public static IObservable<ResetResponse> Reset()
        {
            return _getWebApiRequester().Post<ResetResponse>("Expedition/Reset");
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
