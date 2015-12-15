using System;
using Noroshi.Core.WebApi.Response;
using Noroshi.WebApi;
using UniRx;
using UniLinq;
using Noroshi.Core.Game.Character;
using Noroshi.Datas.Request;

namespace Noroshi.Repositories.Server
{
    public class PlayerCharacterRepository : PlayerDataRepository<PlayerCharacter>
    {
        private WebApiRequester _webApiRequester;

        public override IObservable<PlayerCharacter> Get(uint id)
        {
            return GetAll().Select(ts => ts.FirstOrDefault(t => t.ID == id));
        }

        public override IObservable<PlayerCharacter[]> GetAll()
        {
            return GetData();
        }

        public IObservable<PlayerCharacterChangeGearResponse> EquipGear(uint playerCharacterId, uint gearId, byte index)
        {
            var gear = GlobalContainer.RepositoryManager.GearRepository.Get(gearId);
            if (index > Constant.LAST_GEAR_EQUIP_POSITION || index < Constant.FIRST_GEAR_EQUIP_POSITION || gear == null) {
                throw new InvalidOperationException();
            }
            var sendData = new PlayerCharacterEquipGearRequest { PlayerCharacterID = playerCharacterId, Index = index, GearID = gearId };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerCharacterEquipGearRequest, PlayerCharacterChangeGearResponse>(_url() + "EquipCharacter", sendData);
        }

        public IObservable<PlayerCharacterAndPlayerItemsResponse> UpPromotionLevel(uint playerCharacterId)
        {
            var sendData = new PlayerCharacterUpLevelRequest
            {
                PlayerCharacterID = playerCharacterId,
            };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerCharacterUpLevelRequest, PlayerCharacterAndPlayerItemsResponse>(_url() + "UpPromotionLevel", sendData);
        }

        public IObservable<PlayerCharacterAndStatusResponse> UpActionLevel(uint playerCharacterId, ushort level, byte index)
        {
            var sendData = new PlayerCharacterUpActionLevelRequest
            {
                PlayerCharacterID = playerCharacterId,
                Level = level,
                Index = index
            };
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Post<PlayerCharacterUpActionLevelRequest, PlayerCharacterAndStatusResponse>(_url() + "UpActionLevel", sendData);
        }

        public IObservable<PlayerCharacter[]> GetAllCharacters()
        {
            _webApiRequester = _webApiRequester ?? new WebApiRequester();
            return _webApiRequester.Request<PlayerCharacter[]>(_url() + "GetAllCharacters");
        }


    protected override string _url()
        {
            return "PlayerCharacter/";
        }

        protected override IObservable<PlayerCharacter[]> GetData()
        {
            _webApiRequester = new WebApiRequester();
            return _webApiRequester.Request<PlayerCharacter[]>(_url() + "Get");
        }
    }
}