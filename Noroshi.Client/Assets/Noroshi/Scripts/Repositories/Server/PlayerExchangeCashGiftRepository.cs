using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerExchangeCashGiftRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "ExchangeCashGift/";
        }

        public IObservable<Core.WebApi.Response.PlayerExchangeCashGift[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerExchangeCashGift[]>(_url() + "GetAll");

        }
    }
}
