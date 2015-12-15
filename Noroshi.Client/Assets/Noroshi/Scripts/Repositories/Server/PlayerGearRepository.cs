using Noroshi.Core.WebApi.Request.Item;
using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerGearRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "Gear/";
        }

        public IObservable<Core.WebApi.Response.PlayerGear[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerGear[]>(_url() + "GetAll");

        }

        public IObservable<Core.WebApi.Response.GearCraft> GearCraft(uint gearId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new GearCraftRequest {GearID = gearId};
            return _webApiRequester.Post<GearCraftRequest, Core.WebApi.Response.GearCraft>(_url() + "GearCraft", sendData);

        }

    }
}
