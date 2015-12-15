using System;
using Noroshi.Core.WebApi.Response;
using Noroshi.WebApi;
using UniRx;
using Noroshi.Datas.Request;


namespace Noroshi.Repositories.Server
{
    public class PlayerStatusRepository : PlayerDataRepository<PlayerStatus>
    {
        protected override string _url()
        {
            return "PlayerStatus/";
        }

        public IObservable<PlayerStatus> Get()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerStatus>(_url() + "Get");

        }

        public override IObservable<PlayerStatus> Get(uint id)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusIDRequest() {ID = id};
            return _webApiRequester.Post<PlayerStatusIDRequest, PlayerStatus>(_url() + "GetOther", sendData);
        }

        public IObservable<PlayerStatusLevelUpResponse> AddExp(uint exp)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusExpRequest { Exp = exp };
            return _webApiRequester.Post<PlayerStatusExpRequest, PlayerStatusLevelUpResponse>(_url() + "AddExp", sendData);
        }

        public IObservable<PlayerStatus> AddVipExp(uint exp)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusExpRequest { Exp = exp };
            return _webApiRequester.Post<PlayerStatusExpRequest, PlayerStatus>(_url() + "AddVipExp", sendData);
        }

        public IObservable<PlayerStatus> AddFreeGem(uint gem)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusAddGemRequest { Gem = gem };
            return _webApiRequester.Post<PlayerStatusAddGemRequest, PlayerStatus>(_url() + "AddFreeGem", sendData);
        }

        public IObservable<PlayerStatus> AddChargeGem(uint gem)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusAddGemRequest { Gem = gem };
            return _webApiRequester.Post<PlayerStatusAddGemRequest, PlayerStatus>(_url() + "AddChargeGem", sendData);
        }

        public IObservable<PlayerStatus> ChangeAvaterCharacterID(ushort id)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusAvaterIDRequest { ID = id };
            return _webApiRequester.Post<PlayerStatusAvaterIDRequest, PlayerStatus>(_url() + "ChangeAvaterCharacterID", sendData);
        }

        public IObservable<PlayerStatus> UseGemWithPlayer(uint gem, ushort count, byte type)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusUseGemRequest {Count = count, Gem = gem, Type = type};
            return _webApiRequester.Post<PlayerStatusUseGemRequest, PlayerStatus>(_url() + "UseGemWithPlayer", sendData);
        }

        public IObservable<PlayerStatus> ChangeName(string name)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeNameRequest { Name = name };
            return _webApiRequester.Post<PlayerStatusChangeNameRequest, PlayerStatus>(_url() + "ChangeName", sendData);
        }

        public override IObservable<PlayerStatus[]> GetAll()
        {
            throw new Exception();
        }

        protected virtual IObservable<PlayerStatus[]> GetData()
        {
            throw new Exception();
        }
    }
}