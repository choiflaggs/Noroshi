using Noroshi.Core.WebApi.Request.Item;
using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerDrugRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "Drug/";
        }

        public IObservable<Core.WebApi.Response.PlayerDrug[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerDrug[]>(_url() + "GetAll");

        }

        public IObservable<Core.WebApi.Response.UseDrugWithCharacterResponse> UseDrugWithCharacter(uint drugId, uint characterId, ushort usePossessionsCount)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var requestData = new UseDrugRequest {DrugID = drugId, CharacterID = characterId, UsePossessionsCount = usePossessionsCount };
            return _webApiRequester.Post<UseDrugRequest, Core.WebApi.Response.UseDrugWithCharacterResponse>(_url() + "UseDrugWithCharacter", requestData);
        }
    }
}
