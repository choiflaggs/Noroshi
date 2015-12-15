using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerGearEnchantMaterialRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "GearEnchantMaterial/";
        }

        public IObservable<Core.WebApi.Response.PlayerGearEnchantMaterial[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerGearEnchantMaterial[]>(_url() + "GetAll");

        }
    }
}
