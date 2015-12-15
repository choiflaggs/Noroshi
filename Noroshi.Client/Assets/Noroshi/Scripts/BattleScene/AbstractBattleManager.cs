using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.BattleScene.Drop;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;
using CameraShakeByActionType = Noroshi.BattleScene.Actions.CameraShakeByActionType;

namespace Noroshi.BattleScene
{
    public enum BattleType
    {
        CpuBattle,
        PlayerBattle,
    }
    public abstract class AbstractBattleManager : IBattleManager
    {
        /// バトルカテゴリー。
        protected readonly BattleCategory _battleCategory;
        /// バトルコンテンツ ID。
        protected readonly uint _battleContentId;
        /// バトルに持ち込んだ自プレイヤーキャラクター ID。
        protected readonly uint[] _ownPlayerCharacterIds;
        /// バトルに持ち込んだ傭兵プレイヤーキャラクター ID。
        protected readonly uint? _rentalPlayerCharacterId;
        /// 支払い数（BP など）。
        protected readonly uint _paymentNum;
        /// 次ウェーブ準備の完了時に準備完了ウェーブ番号が OnNext される Subject。
        readonly Subject<byte> _onCompletePrepareNextWave = new Subject<byte>();
        /// バトル終了時に結果が OnNext される Subject。
        readonly Subject<VictoryOrDefeat> _onFinishBattleSubject = new Subject<VictoryOrDefeat>();
        /// 次ウェーブへの遷移試行時に true が OnNext される Subject。
        readonly Subject<bool> _onTryToTransitToNextWaveSubject = new Subject<bool>();
        /// キャラクターエフェクト操作時に操作内容が OnNext される Subject。
        readonly Subject<CharacterEffectEvent> _onCommandCharacterEffectSubject = new Subject<CharacterEffectEvent>();
        /// サウンド操作時に操作内容が OnNext される Subject。
        readonly Subject<SoundEvent> _onCommandSoundSubject = new Subject<SoundEvent>();
        /// カメラ操作時に操作内容が OnNext される Subject。
        readonly Subject<CameraShakeByActionType> _onTryCameraShakeSubject = new Subject<CameraShakeByActionType>();
        /// ウェーブバトル内でのカウントダウン（1秒毎）で残り時間が OnNext される Subject。
        readonly Subject<byte> _onCountDownSubject = new Subject<byte>();
        /// 次ウェーブへの遷移用ボタン表示時に true が、非表示時に false が OnNext される Subject。
        readonly Subject<bool> _onToggleNextWaveButtonVisible = new Subject<bool>();
        Subject<float> _onSlowSubject = new Subject<float>();
        Subject<VictoryOrDefeat> _onEnterWinCharacterAnimationSubject = new Subject<VictoryOrDefeat>();
        Subject<bool> _onWaitRetrySubject = new Subject<bool>();
        Subject<bool> _onSuccessSendResltSubject = new Subject<bool>();
        BattleStateHandler _stateHandler = new BattleStateHandler();

        bool _isInPause = false;
        Subject<bool> _onExitPause;

        protected Core.WebApi.Response.Battle.IBattleStartResponse _startResponse;

        protected List<Wave> _waves = new List<Wave>();
        public DropHandler DropHandler { get; protected set; }
        bool _autoMode = false;
        public BattleResult BattleResult { get; private set; }
        IFieldView _fieldView;
        protected CompositeDisposable _disposables = new CompositeDisposable();
        public Noroshi.Core.WebApi.Response.Battle.AdditionalInformation AdditionalInformation{ get { return _startResponse.AdditionalInformation; } }

        /// 現在の Wave 番号
        public byte CurrentWaveNo { get; private set; }

        public AbstractBattleManager(BattleCategory battleCategory, uint battleContentId, uint[] ownPlayerCharacterIds, uint paymentNum)
        {
            _battleCategory = battleCategory;
            _battleContentId = battleContentId;
            _ownPlayerCharacterIds = ownPlayerCharacterIds;
            _rentalPlayerCharacterId = null;
            _paymentNum = paymentNum;
        }

        public void Initialize()
        {
        }

        public abstract IObservable<IManager> LoadDatas();
        
        protected void _onLoadDatas(IBattleStartResponse data, IBattle battle, BattleCharacter[] ownCharacters)
        {
            _startResponse = data;
            SceneContainer.GetCharacterManager().SetCurrentOwnPlayerCharacters(ownCharacters);

            var necessaryWaveNumForLoopBattle = Constant.LOOP_BATTLE_TIME / Constant.MIN_WAVE_BATTLE + 1;
            var loopNum = LoopBattle ? necessaryWaveNumForLoopBattle / battle.Waves.Count() + 1 : 1;

            var waveNo = 1;
            for (var i = 0; i < loopNum; i++)
            {
                foreach (var wave in battle.Waves)
                {
                    _waves.Add(new Wave(wave, (byte)waveNo++));
                }
            }
            CurrentWaveNo = 1;
            var dropGoldCharacterNum = (byte)battle.Waves.Sum(wave => wave.BattleCharacters.Count());
            DropHandler = new DropHandler(data.DropPossessionObjects, battle.Gold, dropGoldCharacterNum);
        }
        
        public IObservable<IBattleFinishResponse> SendResult()
        {
            IObservable<IBattleFinishResponse> battleFinishResponseObservable = null;
            battleFinishResponseObservable = _sendResult()
            .Do(res => 
            {
                BattleResult.SetFinishResponse(res);
                _onSuccessSendResltSubject.OnNext(true);
                _onSuccessSendResltSubject.OnCompleted();
                _onWaitRetrySubject.OnCompleted();
            })
            .Catch((WWWErrorException e) => 
            {
                _onWaitRetrySubject.OnNext(true);
                return Observable.Empty<IBattleFinishResponse>();
            });

            return battleFinishResponseObservable.PublishLast().RefCount();
        }
        protected abstract IObservable<IBattleFinishResponse> _sendResult();
        
        /// サーバーへ送信するバトル結果を生成。
        protected Core.Game.Battle.BattleResult _makeResult()
        {
            // 自プレイヤー状態。
            var ownPlayerCharacters = SceneContainer.GetCharacterManager().GetCurrentOwnCharacters().Select(c => new AfterBattlePlayerCharacter
            {
                PlayerCharacterID = ((PlayerCharacter)c).PlayerCharacterID,
                HP = (uint)c.CurrentHP,
                Energy = (ushort)c.Energy.Current,
            }).ToArray();
            return new Core.Game.Battle.BattleResult()
            {
                AfterBattlePlayerCharacters = ownPlayerCharacters,
                CurrentWaveNo = CurrentWaveNo,
                CurrentWave = new Core.Game.Battle.BattleResult.Wave
                {
                    EnemyCharacterStates = SceneContainer.GetCharacterManager().CurrentEnemyCharacterSet.GetCharacters()
                        .Select(c => new Core.Game.Battle.BattleResult.EnemyCharacterState { InitialHP = c.HP.InitialHP, RemainingHP = (uint)c.CurrentHP })
                        .ToArray(),
                }
            };
        }

        public virtual IObservable<IManager> LoadAssets(IFactory factory)
        {
            return factory.BuildFieldView(FieldID)
            .Select(v =>
            {
                _fieldView = v;
                return (IManager)this;
            });
        }

        public abstract uint CharacterExp { get; }
        public abstract uint FieldID { get; }

        public BattleAutoMode BattleAutoMode { get { return (BattleAutoMode)_startResponse.BattleAutoMode; } }
        /// ループバトルフラグ。
        public bool LoopBattle { get { return _startResponse.LoopBattle; } }
        public ushort PlayerExp { get { return _startResponse.PlayerExp; } }

        /// 次ウェーブ準備の完了時に準備完了ウェーブ番号が OnNext される Observable を取得。
        public IObservable<byte> GetOnCompletePrepareNextWaveObservable() { return _onCompletePrepareNextWave.AsObservable(); }
        /// バトル終了時に結果が OnNext される Observable を取得。
        public IObservable<VictoryOrDefeat> GetOnFinishBattleObservable() { return _onFinishBattleSubject.AsObservable(); }
        /// キャラクターエフェクト操作時に操作内容が OnNext される Observable を取得。
        public IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable() { return _onCommandCharacterEffectSubject.AsObservable(); }
        /// サウンド操作時に操作内容が OnNext される Observable を取得。
        public IObservable<SoundEvent> GetOnCommandSoundObservable() { return _onCommandSoundSubject.AsObservable(); }
        /// カメラ操作時に操作内容が OnNext される Observable を取得。
        public IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable() { return _onTryCameraShakeSubject.AsObservable(); }
        /// ウェーブバトル内でのカウントダウン（1秒毎）で残り時間が OnNext される Observable を取得。
        public IObservable<byte> GetOnCountDownObservable() { return _onCountDownSubject.AsObservable(); }
        /// 次ウェーブへの遷移用ボタン表示時に true が、非表示時に false が OnNext される Observable を取得。
        public IObservable<bool> GetOnToggleNextWaveButtonVisibleObservable() { return _onToggleNextWaveButtonVisible.AsObservable(); }
        public IObservable<float> GetOnSlowObservable() { return _onSlowSubject.AsObservable(); }
        public IObservable<VictoryOrDefeat> GetOnEnterWinCharacterAnimation() { return _onEnterWinCharacterAnimationSubject.AsObservable(); }
        public IObservable<bool> GetOnWaitRetryObservable() { return _onWaitRetrySubject.AsObservable(); }

        /// 最後の Wave 番号
        public int WaveNum { get { return _waves.Count(); } }
        /// 現在の Wave
        public Wave CurrentWave { get { return _waves[CurrentWaveNo - 1]; } }

        public IObservable<byte> GetOnEnterWaveBattleObservable()
        {
            return _stateHandler.GetOnEnterStateObservable().Where(t => t == typeof(BattleStateHandler.Wave)).Select(_ => CurrentWaveNo);
        }

        /// バトルを準備。
        public virtual void Prepare()
        {
            CurrentWave.SetCharactersToInitialPosition();
            if (BattleAutoMode == BattleAutoMode.Auto)
            {
                SetAutoMode(true);
                SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.SetAuto(true);
            }
            DropHandler.GetOnCommandDropMoneyObservable().Subscribe(e => {
                _onCommandCharacterEffectSubject.OnNext(new CharacterEffectEvent
                {
                    Command = CharacterEffectCommand.PlayOnce,
                    CharacterView = e.CharacterView,
                    CharacterEffectID = Constant.DROP_COIN_CHARACTER_EFFECT_ID,
                });
            }).AddTo(_disposables);
        }
        protected abstract uint _getSoundId();

        public IObservable<VictoryOrDefeat> Start()
        {
            // 各 Wave の Observable に処理を紐付ける。

            _waves.Select(w => w.GetOnFinishBattleObservable()).Merge()
            .Subscribe(_onFinishWaveBattle).AddTo(_disposables);

            _waves.Select(w => w.GetOnCountDownObservable()).Merge()
            .Subscribe(_onCountDownSubject.OnNext).AddTo(_disposables);

            _waves.Select(w => w.GetOnCharacterDieObservable()).Merge()
            .Subscribe(_onCharacterDie).AddTo(_disposables);

            _waves.Select(w => w.GetOnCommandCharacterEffectObservable()).Merge()
            .Subscribe(_onCommandCharacterEffectSubject.OnNext).AddTo(_disposables);

            _waves.Select(w => w.GetOnCommandSoundObservable()).Merge()
            .Subscribe(_onCommandSoundSubject.OnNext).AddTo(_disposables);

            _waves.Select(w => w.GetOnDarkenFieldObservable()).Merge()
            .Subscribe(_onDarkenField).AddTo(_disposables);

            _waves.Select(w => w.GetOnTryCameraShakeObservable()).Merge()
            .Subscribe(_onTryCameraShakeSubject.OnNext).AddTo(_disposables);

            _onCommandSoundSubject.OnNext(new SoundEvent()
            {
                Command = SoundCommand.Play,
                SoundID = _getSoundId(),
            });

            // 状態遷移開始
            return _stateHandler.Start()
                .Do(result => {
                    BattleResult = new BattleResult(result);
                    BattleResult.GetOnEnterWinCharacterAnimation()
                    .Subscribe(victoryOrDefeat =>
                    {
                        DropHandler.ForcePickUpDropItems();
                        _onEnterWinCharacterAnimationSubject.OnNext(victoryOrDefeat);
                    });
                })
                .SelectMany(br => _waitForBattleFinish().Select(_ => br))
                .SelectMany(br => _pauseAtFinish(br).Select(_ => br))
                .SelectMany(br => BattleResult.PlayCharacterWinAnimation())
                .SelectMany(br => 
                {
                    SendResult().Subscribe().AddTo(_disposables);
                    return _onSuccessSendResltSubject.AsObservable().Select(_ => br);
                });
        }
        /// キャラクター死亡時処理。
        protected virtual void _onCharacterDie(Character character)
        {
            // 死亡キャラクターが敵の場合はドロップ処理
            if (character.GetType() != typeof(ShadowCharacter) && character.Force == Force.Enemy)
            {
                DropHandler.Drop(character.GetView(), CurrentWaveNo, character.No);
            }
        }
        /// フィールド暗転、暗転解除時処理。
        void _onDarkenField(bool darken)
        {
            if (darken)
            {
                _fieldView.Darken();
            }
            else
            {
                _fieldView.Brighten();
            }
        }
        /// ウェーブバトル終了時処理。
        void _onFinishWaveBattle(VictoryOrDefeat waveBattleResult)
        {
            // 次ウェーブがある場合。
            if (waveBattleResult == VictoryOrDefeat.Win && CurrentWaveNo < WaveNum)
            {
                var ownCharacters = SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.GetCharacters();
                foreach (var ownCharacter in ownCharacters)
                {
                    ownCharacter.CheckActiveActionAvailable();
                }
                _stateHandler.TransitToInterval();
            }
            // 次ウェーブがない（バトル終了の）場合。
            else
            {
                _stateHandler.Finish(waveBattleResult);
            }
        }

        IObservable<bool> _waitForBattleFinish()
        {
            return _isInPause ? _onExitPause.AsObservable() : Observable.Return<bool>(true);
        }
        IObservable<bool> _pauseAtFinish(VictoryOrDefeat battleResult)
        {
            _onTryCameraShakeSubject.OnCompleted();
            _onCommandCharacterEffectSubject.OnCompleted();
            _onFinishBattleSubject.OnNext(battleResult);
            return _pauseWithTimeSpan(Constant.BATTLE_FINISH_PAUSE_TIME).Do(_ => _onPauseAtFinish());
        }
        protected virtual void _onPauseAtFinish()
        {
            // スロー開始
            _onSlowSubject.OnNext(Constant.BATTLE_FINISH_SLOW_TIME);
        }

        public IObservable<IBattleManager> RunOwnCharactersToCorrectPosition()
        {
            return CurrentWave.RunOwnCharactersToCorrectPosition(GetSwitchWaveTime()).Select(_ => (IBattleManager)this);
        }

        /// Ready 状態の終了を待つべきかどうか判定。
        public virtual bool ShouldWaitToFinishReady() { return false; }

        public virtual void StartWave()
        {
            CurrentWave.Start(Constant.WAVE_TIME);
        }
        public IObservable<IBattleManager> StartInterval()
        {
            _onToggleNextWaveButtonVisible.OnNext(true);
            return _getOnCommandTransitToNextWaveObservable()
            .SelectMany(_ =>
            {
                SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.RecoveryAtIntervalEnter();
                DropHandler.ForcePickUpDropItems();
                _onToggleNextWaveButtonVisible.OnNext(false);
                return SceneContainer.GetTimeHandler().Timer(1);
            })
            .Select(_ => (IBattleManager)this);
        }
        public virtual IObservable<Wave> SwitchWave()
        {
            CurrentWave.Dispose();

            SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.PrepareNextWave();

            CurrentWaveNo++;
            _onCompletePrepareNextWave.OnNext(CurrentWaveNo);
            SceneContainer.GetCharacterManager().SwitchNextEnemyCharacterSet();
            return _setCharactersToInitialPositionWithAnimation();
        }
        IObservable<Wave> _setCharactersToInitialPositionWithAnimation()
        {
            return CurrentWave.SetCharactersToInitialPositionWithAnimation(GetSwitchWaveTime(), _isMovingToNextWaveWithWalking());
        }
        protected virtual bool _isMovingToNextWaveWithWalking()
        {
            return false;
        }
        public float GetSwitchWaveTime()
        {
            return _isMovingToNextWaveWithWalking() ? Constant.SLOW_SWITCH_WAVE_TIME : Constant.SWITCH_WAVE_TIME;
        }

        public void TryToTransitToNextWave() { _onTryToTransitToNextWaveSubject.OnNext(false); }
        IObservable<bool> _getOnCommandTransitToNextWaveObservable()
        {
            return _autoMode ? Observable.Return<bool>(true) : _onTryToTransitToNextWaveSubject.AsObservable();
        }

        IObservable<bool> _pauseWithTimeSpan(float timeSpan)
        {
            // 指定時間後に解除設定。
            SceneContainer.GetTimeHandler().Timer(timeSpan).Subscribe(_ => _exitPause());
            return _enterPause();
        }
        protected IObservable<bool> _enterPause()
        {
            _isInPause = true;
            var characters = SceneContainer.GetCharacterManager().GetCurrentAllCharacters();
            foreach (var character in characters)
            {
                character.PauseOn();
            }
            var onExitPause = new Subject<bool>();
            _onExitPause = onExitPause;
            return onExitPause.AsObservable();
        }
        protected void _exitPause()
        {
            var characters = SceneContainer.GetCharacterManager().GetCurrentAllCharacters();
            foreach (var character in characters)
            {
                character.PauseOff();
            }
            _isInPause = false;
            _onExitPause.OnNext(true);
        }

        public void LogicUpdate()
        {
            if (!_stateHandler.CanLogicUpdate()) return;
            foreach (var character in CurrentWave.Field.GetAllCharacters())
            {
                character.LogicUpdate();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _stateHandler.Dispose();
            CurrentWave.Dispose();
            DropHandler.Dispose();
        }

        /// オートモード切り替え。
        public void SetAutoMode(bool isOn)
        {
            _autoMode = isOn;
        }

        /// バトル結果 UI でピックアップするキャラクターを取得する。
        public Character GetPickUpCharacter()
        {
            return BattleResult.GetVictoryOrDefeat() == VictoryOrDefeat.Win
                ? SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.GetPickUpCharacter()
                : SceneContainer.GetCharacterManager().CurrentEnemyCharacterSet.GetPickUpCharacter();
        }
    }
}
