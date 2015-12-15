using Noroshi.Core.WebApi.Response.Arena;
using Noroshi.Core.WebApi.Response.Players;
using Noroshi.Datas.Request;
using Noroshi.WebApi;
using UniRx;

namespace Noroshi.Repositories.Server
{
    public class PlayerArenaRepository : PlayerDataRepository<PlayerArena>
    {

        public IObservable<PlayerArena> Get()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerArena>(_url() + "Get");
        }

        public IObservable<PlayerArenaOtherResponse> GetOther(uint playerId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var queryData = new PlayerArenaOtherPlayerRequest {OtherPlayerID = playerId};
            return _webApiRequester.Post<PlayerArenaOtherPlayerRequest, PlayerArenaOtherResponse>(_url() + "Get", queryData);
        }

        public IObservable<PlayerArenaOtherResponse[]> GetRandomPlayers()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerArenaOtherResponse[]>(_url() + "GetRandomPlayers");

        }

        public IObservable<PlayerArena> ChangeDeck(uint[] characterIds)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var queryData = new PlayerArenaChangeDeckRequest {CharacterIDs = characterIds};
            return _webApiRequester.Post<PlayerArenaChangeDeckRequest, PlayerArena>(_url() + "ChangeDeck", queryData);
        }


        protected string _url()
        {
            return "PlayerArena/";
        }

    }
}