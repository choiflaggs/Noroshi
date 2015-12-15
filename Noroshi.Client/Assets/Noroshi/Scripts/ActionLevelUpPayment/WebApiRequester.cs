using UniRx;

namespace Noroshi.ActionLevelUpPayment
{
    class WebApiRequester
    {
        protected string _url()
        {
            return "ActionLevelUpPayment/";
        }

        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }

        public IObservable<Core.WebApi.Response.ActionLevelUpPayment[]> MasterData()
        {
            return _getWebApiRequester().Request<Core.WebApi.Response.ActionLevelUpPayment[]>(_url() + "MasterData");
        }
    }
}
