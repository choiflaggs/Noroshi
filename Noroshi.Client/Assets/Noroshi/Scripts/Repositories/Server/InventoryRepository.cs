using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class InventoryRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "Inventory/";
        }

        public IObservable<Core.WebApi.Response.Inventory> Get()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.Inventory>(_url() + "Get");

        }
    }
}
