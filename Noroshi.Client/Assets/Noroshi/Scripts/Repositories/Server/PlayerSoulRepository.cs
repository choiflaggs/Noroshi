using Noroshi.Datas.Request;
using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerSoulRepository
    {
        private WebApiRequester _webApiRequester;

        protected string _url()
        {
            return "Soul/";
        }

        public IObservable<Core.WebApi.Response.PlayerSoul[]> GetAll()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<Core.WebApi.Response.PlayerSoul[]>(_url() + "GetAll");
        }

        public IObservable<Core.WebApi.Response.Character.CreateCharacter> UseSoulWithCreateCharacter(uint soulId)
        {
            var sendData = new PlayerSoulCreateCharacterRequest
            {
                SoulID = soulId
            };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerSoulCreateCharacterRequest, Core.WebApi.Response.Character.CreateCharacter>(_url() + "UseSoulWithCreateCharacter", sendData);
        }

        public IObservable<Core.WebApi.Response.CharacterEvolutionLevelUpResponse> UseSoulWithEvolutionLevel(uint soulId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerSoulCreateCharacterRequest
            {
                SoulID = soulId
            };
            return _webApiRequester.Post<PlayerSoulCreateCharacterRequest, Core.WebApi.Response.CharacterEvolutionLevelUpResponse>(_url() + "UseSoulWithEvolutionLevel", sendData);
        }

    }
}
