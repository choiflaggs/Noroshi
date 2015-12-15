using Noroshi.Core.WebApi.Response;
using Noroshi.NoroshiDebug.Datas.Request;
using Noroshi.WebApi;
using UniRx;
using Noroshi.Repositories.Server;

namespace NoroshiDebug.Repositories.Server
{
    public class PlayerItemDebugRepository : PlayerDataRepository<PlayerItem>
    {
        private WebApiRequester _webApiRequester;

        public IObservable<PlayerItem> AddItem(uint itemId, ushort addItemCount)
        {
            var sendData = new PlayerItemGetRequest { ItemID = itemId, AddPossessionsCount = addItemCount };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerItemGetRequest, PlayerItem>(_url() + "GetItem", sendData);
        }

        public IObservable<PlayerItem> UseItem(uint itemId, ushort useItemCount)
        {
            var sendData = new PlayerItemUseRequest { ItemID = itemId, UsePossessionsCount = useItemCount };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerItemUseRequest, PlayerItem>(_url() + "UseItem", sendData);
        }


        protected override string _url()
        {
            return "PlayerItemDebug/";
        }
    }
}