using Noroshi.Core.WebApi.Response;
using Noroshi.WebApi;
using UniRx;
using Noroshi.NoroshiDebug.Datas.Request;
using Noroshi.Repositories.Server;

namespace NoroshiDebug.Repositories.Server
{
    public class PlayerStatusDebugReporitosy : PlayerDataRepository<PlayerStatus>
    {
        private WebApiRequester _webApiRequester;

        public IObservable<PlayerStatus> ChangeExp(uint exp)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeExp { Exp = exp };
            return _webApiRequester.Post<PlayerStatusChangeExp, PlayerStatus>(_url() + "ChangeExp", sendData);
        }

        public IObservable<PlayerStatus> ChangeLevel(ushort level)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeLevel { Level = level };
            return _webApiRequester.Post<PlayerStatusChangeLevel, PlayerStatus>(_url() + "ChangeLevel", sendData);
        }
        public IObservable<PlayerStatus> ChangeGold(uint gold)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeGold { Gold = gold };
            return _webApiRequester.Post<PlayerStatusChangeGold, PlayerStatus>(_url() + "ChangeGold", sendData);
        }
        public IObservable<PlayerStatus> ChangeGem(uint gem)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeGem { Gem = gem };
            return _webApiRequester.Post<PlayerStatusChangeGem, PlayerStatus>(_url() + "ChangeGem", sendData);
        }
        public IObservable<PlayerStatus> ChangeStamina(ushort stamina)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerStatusChangeStamina { Stamina = stamina };
            return _webApiRequester.Post<PlayerStatusChangeStamina, PlayerStatus>(_url() + "ChangeStamina", sendData);
        }


        protected override string _url()
        {
            return "PlayerStatus/";
        }
    }
}