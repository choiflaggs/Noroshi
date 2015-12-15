using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;

namespace Noroshi.BattleScene
{
    public class PlayerBattleManager : AbstractBattleManager
    {
        public PlayerBattleManager(BattleCategory battleCategory, uint battleContentId, uint[] ownPlayerCharacterIds, uint paymentNum) : base(battleCategory, battleContentId, ownPlayerCharacterIds, paymentNum)
        {
        }

        public override uint CharacterExp { get { return ((Core.WebApi.Response.Battle.PlayerBattleStartResponse)_startResponse).Battle.CharacterExp; } }
        public override uint FieldID { get { return ((Core.WebApi.Response.Battle.PlayerBattleStartResponse)_startResponse).Battle.FieldID; } }

        public override IObservable<IManager> LoadDatas()
        {
            // バトル開始用データをロード
            return _requestPlayerBattleStartWebAPI()
            .Do(response =>
            {
                SceneContainer.GetCharacterManager().SetCurrentEnemyPlayerCharacterSets(response.Battle.Waves.Select(w => w.BattleCharacters.Where(c => c != null)));
                _onLoadDatas(response, response.Battle, response.OwnCharacters);
            })
            .Select(_ => (IManager)this);
        }
        IObservable<PlayerBattleStartResponse> _requestPlayerBattleStartWebAPI()
        {
            var requestParam = new Datas.Request.PlayerBattleStartRequest
            {
                Category = (byte)_battleCategory,
                ID = _battleContentId,
                PlayerCharacterIDs = _ownPlayerCharacterIds,
                RentalPlayerCharacterID = _rentalPlayerCharacterId,
                PaymentNum = _paymentNum,
            };
            return SceneContainer.GetWebApiRequester().Post<Datas.Request.PlayerBattleStartRequest, PlayerBattleStartResponse>("/Battle/StartPlayerBattle", requestParam);
        }
        protected override IObservable<IBattleFinishResponse> _sendResult()
        {
            var requestParam = new Datas.Request.PlayerBattleFinishRequest(){
                Category = (byte)_battleCategory,
                ID = _battleContentId,
                VictoryOrDefeat = (byte)BattleResult.GetVictoryOrDefeat(),
                Rank = BattleResult.GetRank(),
                Result = LitJson.JsonMapper.ToJson(_makeResult()),
            };
            return SceneContainer.GetWebApiRequester().Post<Datas.Request.PlayerBattleFinishRequest, PlayerBattleFinishResponse>("/Battle/FinishPlayerBattle", requestParam)
            .Cast<PlayerBattleFinishResponse, IBattleFinishResponse>();
        }
        protected override uint _getSoundId()
        {
            return Constant.PLAYER_BATTLE_BGM_SOUND_ID;
        }
    }
}
