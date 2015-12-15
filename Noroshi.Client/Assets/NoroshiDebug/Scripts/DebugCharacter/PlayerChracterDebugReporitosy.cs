using Noroshi.Core.WebApi.Response;
using Noroshi.WebApi;
using UniRx;
using System.Collections.Generic;
using Noroshi.NoroshiDebug.Datas.Request;
using Noroshi.Repositories.Server;

namespace NoroshiDebug.Repositories.Server
{
    public class PlayerCharacterDebugReporitosy : PlayerDataRepository<PlayerCharacter>
    {
        private List<PlayerCharacter> _playerCharacter;
        private WebApiRequester _webApiRequester;

        public IObservable<PlayerCharacter> AllEquipGear(uint playerCharacterId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangeEquip { PlayerCharacterID = playerCharacterId };
            return _webApiRequester.Post<PlayerCharacterChangeEquip, PlayerCharacter>(_url() + "AllEquip", sendData);
        }

        public IObservable<PlayerCharacter> RemoveEquipGear(uint playerCharacterId)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangeEquip { PlayerCharacterID = playerCharacterId };
            return _webApiRequester.Post<PlayerCharacterChangeEquip, PlayerCharacter>(_url() + "RemoveEquip", sendData);
        }

        public IObservable<PlayerCharacter> ChangedLevel(uint playerCharacterId, ushort level)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangedLevelRequest { PlayerCharacterID = playerCharacterId, Level = level };
            return _webApiRequester.Post<PlayerCharacterChangedLevelRequest, PlayerCharacter>(_url() + "ChangeLevel", sendData);
        }

        public IObservable<PlayerCharacter> ChangePromotionLevel(uint playerCharacterId, byte level)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangeSubLevelRequest { PlayerCharacterID = playerCharacterId, Level = level };
            return _webApiRequester.Post<PlayerCharacterChangeSubLevelRequest, PlayerCharacter>(_url() + "ChangePromotionLevel", sendData);
        }

        public IObservable<PlayerCharacter> ChangeEvolutionLevel(uint playerCharacterId, byte level)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangeSubLevelRequest { PlayerCharacterID = playerCharacterId, Level = level };
            return _webApiRequester.Post<PlayerCharacterChangeSubLevelRequest, PlayerCharacter>(_url() + "ChangeEvolutionLevel", sendData);
        }

        public IObservable<PlayerCharacter> ChangeActionLevel(uint playerCharacterId, ushort level, ushort index)
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            var sendData = new PlayerCharacterChangeActionLevel { PlayerCharacterID = playerCharacterId, Level = level, Index = index };
            return _webApiRequester.Post<PlayerCharacterChangeActionLevel, PlayerCharacter>(_url() + "ChangeActionLevel", sendData);
        }


        protected override string _url()
        {
            return "PlayerCharacterDebug/";
        }
    }
}