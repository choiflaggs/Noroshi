using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;

namespace Noroshi.BattleScene
{
    public class CpuBattleManager : AbstractBattleManager
    {
        StoryHandler _storyHandler;

        Subject<Story> _onEnterBeforeBossWaveStory = new Subject<Story>();
        Subject<bool> _onExitWaitBeforeBossWaveStory = new Subject<bool>();

        Subject<Story> _onEnterAfterBossDieStory = new Subject<Story>();
        Subject<bool> _onExitWaitAfterBossDieStory = new Subject<bool>();

        Subject<Story> _onEnterAfterBattleStory = new Subject<Story>();
        Subject<bool> _onExitWaitAfterBattleStory = new Subject<bool>();

        bool _hasPlayedBeforeBossWaveStory = false;
        bool _hasPlayedAfterBossDieStory = false;
        bool _hasSlowTimeAtFinish;

        public CpuBattleManager(BattleCategory battleCategory, uint battleContentId, uint[] ownPlayerCharacterIds, uint paymentNum) : base(battleCategory, battleContentId, ownPlayerCharacterIds, paymentNum)
        {
        }

        public override uint CharacterExp { get { return ((Core.WebApi.Response.Battle.CpuBattleStartResponse)_startResponse).Battle.CharacterExp; } }
        public override uint FieldID { get { return ((Core.WebApi.Response.Battle.CpuBattleStartResponse)_startResponse).Battle.FieldID; } }

        public override IObservable<IManager> LoadDatas()
        {
            // バトル開始用データをロード
            return _requestEnemyBattleStartWebAPI()
            .Do(response =>
            {
                SceneContainer.GetCharacterManager().SetCurrentEnemyCpuCharacterSets(response.Battle.Waves.Select(w => w.BattleCharacters.Where(c => c != null)));
                _onLoadDatas(response, response.Battle, response.OwnCharacters);
                _storyHandler = new StoryHandler(response.Battle.BeforeBattleStory, response.Battle.BeforeBossWaveStory, response.Battle.AfterBossDieStory, response.Battle.AfterBattleStory);
            })
            .SelectMany(_ => _storyHandler.LoadDatas())
            .Select(_ => (IManager)this);
        }
        public override IObservable<IManager> LoadAssets(IFactory factory)
        {
            return base.LoadAssets(factory).SelectMany(_storyHandler.LoadAssets(factory)).Select(_ => (IManager)this);
        }

        public string GetTitleTextKey() { return _startResponse.AdditionalInformation != null ? _startResponse.AdditionalInformation.BattleTitleTextKey : ""; }

        public bool HasStory
        {
            get { return _storyHandler.HasStory; }
        }

        public Story GetBeforeBattleStory()
        {
            return _storyHandler.GetBeforeBattleStory();
        }
        public IObservable<Story> GetOnEnterBeforeBossWaveStory()
        {
            return _onEnterBeforeBossWaveStory.AsObservable();
        }
        public IObservable<Story> GetOnEnterAfterBossDieStory()
        {
            return _onEnterAfterBossDieStory.AsObservable();
        }
        public IObservable<Story> GetOnEnterAfterBattleStory()
        {
            return _onEnterAfterBattleStory.AsObservable();
        }

        public override void Prepare()
        {
            base.Prepare();
            // バトル前ストーリーが存在する場合、
            // 演出上、キャラクターの初期ポジションを変える必要がある。
            if (GetBeforeBattleStory() != null)
            {
                foreach (var ownCharacter in SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.GetCharacters())
                {
                    ownCharacter.SetViewToStoryWavePosition();
                }
            }
            _storyHandler.Prepare();
        }

        protected override void _onPauseAtFinish()
        {
            if (_hasSlowTimeAtFinish) base._onPauseAtFinish();
        }

        /// Ready 状態の終了を待つべきかどうか判定。事前ストーリーが存在していれば待つ。
        public override bool ShouldWaitToFinishReady() { return GetBeforeBattleStory() != null; }

        public override void StartWave()
        {
            base.StartWave();
            // 敵のボスキャラクターにはフラグを立てておく。
            // ボス撃破時後ストーリー演出が「死亡ではなく逃走」の場合もフラグを立てておく。
            if (_storyHandler.GetAfterBossDieStory() == null) return;
            var bossEnemyCharacter = SceneContainer.GetCharacterManager().GetCurrentEnemyCharacters()
            .Select(character => (CpuCharacter)character)
            .Where(character => character.IsBoss)
            .FirstOrDefault(); 
            if (bossEnemyCharacter != null)
            {
                var dramaType = _storyHandler.GetAfterBossDieStory().GetDramaType();
                if (dramaType.HasValue && dramaType.Value == Story.DramaType.BossEscape)
                {
                   bossEnemyCharacter.SetEscapeBeforeDeadOn();
                }
            }
        }

        public override IObservable<Wave> SwitchWave()
        {
            return base.SwitchWave().SelectMany(wave => CurrentWaveNo == WaveNum ? _enterBeforeBossWaveStory().Select(_ => wave) : Observable.Return<Wave>(wave));
        }

        protected override bool _isMovingToNextWaveWithWalking()
        {
            return CurrentWave.No == WaveNum && HasStory;
        }

        IObservable<bool> _enterBeforeBossWaveStory()
        {
            if (_storyHandler.GetBeforeBossWaveStory() == null) return Observable.Return<bool>(false);
            if (_hasPlayedBeforeBossWaveStory) return Observable.Return<bool>(false);
            _hasPlayedBeforeBossWaveStory = true;
            _onEnterBeforeBossWaveStory.OnNext(_storyHandler.GetBeforeBossWaveStory());
            _onEnterBeforeBossWaveStory.OnCompleted();
            return _onExitWaitBeforeBossWaveStory.AsObservable();
        }
        public void ExitWaitBeforeBossWaitStory()
        {
            _onExitWaitBeforeBossWaveStory.OnNext(true);
            _onExitWaitBeforeBossWaveStory.OnCompleted();
        }

        IObservable<bool> _enterAfterBossDieStory()
        {
            if (_storyHandler.GetAfterBossDieStory() == null) return Observable.Return<bool>(false);
            if (_hasPlayedAfterBossDieStory) return Observable.Return<bool>(false);
            _hasPlayedAfterBossDieStory = true;
            _enterPause().Subscribe().AddTo(_disposables);
            _onEnterAfterBossDieStory.OnNext(_storyHandler.GetAfterBossDieStory());
            _onEnterAfterBossDieStory.OnCompleted();
            return _onExitWaitAfterBossDieStory.AsObservable();
        }
        public void ExitWaitAfterBossDieStory()
        {
            _onExitWaitAfterBossDieStory.OnNext(true);
            _onExitWaitAfterBossDieStory.OnCompleted();
            _exitPause();
        }

        public IObservable<bool> EnterAfterBattleStory()
        {
            if (_storyHandler.GetAfterBattleStory() == null) return Observable.Return<bool>(false);
            _onEnterAfterBattleStory.OnNext(_storyHandler.GetAfterBattleStory());
            _onEnterAfterBattleStory.OnCompleted();
            return _onExitWaitAfterBattleStory.AsObservable();
        }
        public void ExitWaitAfterBattleStory()
        {
            _onExitWaitAfterBattleStory.OnNext(true);
            _onExitWaitAfterBattleStory.OnCompleted();
        }

        /// キャラクター死亡時処理。
        protected override void _onCharacterDie(Character character)
        {
            base._onCharacterDie(character);

            // 以降は敵キャラクターが死亡した処理。
            if (character.Force != Force.Enemy) return;
            var cpuCharacter = (CpuCharacter)character;
            // ボス撃破後ストーリーが存在し、該当ボスが（死亡の代わりに）逃走演出設定の場合、
            // 死亡時にストーリー開始処理を差し込む。
            if (_storyHandler.GetAfterBossDieStory() != null && cpuCharacter.IsBoss && cpuCharacter.IsDeadEscape)
            {
                _enterAfterBossDieStory().Subscribe(_ => cpuCharacter.EscapeBeforeDead());
                _hasSlowTimeAtFinish = false;
            }
            else
            {
                _hasSlowTimeAtFinish = true;
            }
        }

        IObservable<CpuBattleStartResponse> _requestEnemyBattleStartWebAPI()
        {
            var requestParam = new Datas.Request.CpuBattleStartRequest
            {
                Category = (byte)_battleCategory,
                ID = _battleContentId,
                PlayerCharacterIDs = _ownPlayerCharacterIds,
                RentalPlayerCharacterID = _rentalPlayerCharacterId,
                PaymentNum = _paymentNum,
            };
            return SceneContainer.GetWebApiRequester().Post<Datas.Request.CpuBattleStartRequest, CpuBattleStartResponse>("/Battle/StartCpuBattle", requestParam);
        }
        protected override IObservable<IBattleFinishResponse> _sendResult()
        {
            var requestParam = new Datas.Request.CpuBattleFinishRequest(){
                Category = (byte)_battleCategory,
                ID = _battleContentId,
                VictoryOrDefeat = (byte)BattleResult.GetVictoryOrDefeat(),
                Rank = BattleResult.GetRank(),
                Result = LitJson.JsonMapper.ToJson(_makeResult()),
            };
            return SceneContainer.GetWebApiRequester().Post<Datas.Request.CpuBattleFinishRequest, CpuBattleFinishResponse>("/Battle/FinishCpuBattle", requestParam)
            .Cast<CpuBattleFinishResponse, IBattleFinishResponse>();
        }
        protected override uint _getSoundId()
        {
            return Constant.CPU_BATTLE_BGM_SOUND_ID;
        }
    }
}
