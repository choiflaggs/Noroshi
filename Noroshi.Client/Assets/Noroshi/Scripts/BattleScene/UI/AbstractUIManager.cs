using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene.UI
{
    public abstract class AbstractUIManager : IUIManager
    {
        protected IUIController _uiController;
        PauseModalUI _pauseModalUI;
        ModalUIViewModel _retryModalUI;
        ResultUI _winUI;
        ResultUI _lossUI;
        PlayerLevelUpModalUI _playerLevelUpModalUI;
        Dictionary<byte, DropItemUI> _dropItemUIs = new Dictionary<byte, DropItemUI>();
        OwnCharacterPanelUISet _ownCharacterPanelUISet;
        CharacterAboveUISet _characterAboveUISet = new CharacterAboveUISet();

        Subject<bool> _onTransitScene = new Subject<bool>();
        Subject<bool> _onPauseSubject = new Subject<bool>();
        protected Subject<SoundEvent> _onCommandSoundSubject = new Subject<SoundEvent>();
        protected CompositeDisposable _disposables = new CompositeDisposable();

        public AbstractUIManager()
        {
            _pauseModalUI = new PauseModalUI();
            _retryModalUI = new ModalUIViewModel("RetryModalUI");
            _winUI = new ResultUI("WinUI");
            _lossUI = new ResultUI("LossUI");
            _playerLevelUpModalUI = new PlayerLevelUpModalUI();
        }

        public virtual void Initialize()
        {
            var battleManager = SceneContainer.GetBattleManager();
            var characterManager = SceneContainer.GetCharacterManager();

            // Now Loading 解除
            SceneContainer.GetSceneManager().GetOnExitInitializeObservable()
            .Do(_ => _uiController.DeactiveLoadingUIView())
            .Subscribe().AddTo(_disposables);

            SceneContainer.GetBattleManager().GetOnEnterWaveBattleObservable().SelectMany(waveNo =>
            {
                _setCurrentWaveNum(waveNo);

                _characterAboveUISet.Clear();
                return _characterAboveUISet.LoadCharacterAboveUIViews(_uiController, characterManager.GetCurrentAllCharacters().Where(c => !c.IsDead).ToArray());
            })
            .Subscribe().AddTo(_disposables);
            // 残り時間表示更新
            battleManager.GetOnCountDownObservable().Subscribe(_updateTime)
            .AddTo(_disposables);
            // 次のウェーブへボタンの可視切り替え
            battleManager.GetOnToggleNextWaveButtonVisibleObservable().Subscribe(_setNextWaveButtonVisible)
            .AddTo(_disposables);
            // 勝利時に再生処理を紐付け
            battleManager.GetOnFinishBattleObservable()
            .Do(_ => _playWinMessage())
            .Subscribe().AddTo(_disposables);

            // 勝利モーダルを閉じた際の処理をフック
            GetWinModalUI().GetOnCloseExitObservable().Do(_ =>
            {
                _onTransitScene.OnNext(false);
            })
            .Subscribe().AddTo(_disposables);
            // 敗北モーダルを閉じた際の処理をフック
            GetLossModalUI().GetOnCloseExitObservable().Do(_ =>
            {
                _onTransitScene.OnNext(false);
            })
            .Subscribe().AddTo(_disposables);
            // リザルト時の通信エラー例外をキャッチした際の処理を紐付け
            battleManager.GetOnWaitRetryObservable()
            .Do(_ => _retryModalUI.Open())
            .Subscribe().AddTo(_disposables);

            // 結果モーダル紐付け
            SceneContainer.GetSceneManager().GetOnEnterResultObservable()
            .SelectMany(result => 
            {
                Observable.WhenAll(
                    _uiController.DeactivateHeaderAndFooter(),
                    _uiController.DarkenWorld()
                    ).Subscribe().AddTo(_disposables);
                return _activateBeforeResultUI(result);
            })
            .SelectMany(result =>
            {
                var onExitOpen = result == VictoryOrDefeat.Win ? LoadWinModalUI().SelectMany(modal => modal.Open())
                    : LoadLossModalUI().SelectMany(modal => modal.Open());
                return onExitOpen.SelectMany(_ => {
                    return _activatePlayerLevelUpModalUI(SceneContainer.GetBattleManager().BattleResult.GetAddPlayerExpResult());
                });
            })
            .Subscribe().AddTo(_disposables);
        }
        IObservable<PlayerLevelUpModalUI> _activatePlayerLevelUpModalUI(Core.WebApi.Response.Players.AddPlayerExpResult addPlayerExpResult)
        {
            if (addPlayerExpResult == null || !addPlayerExpResult.LevelUp) return Observable.Return<PlayerLevelUpModalUI>(null);
            return _loadModalUI(_playerLevelUpModalUI).SelectMany(modal => modal.Open(addPlayerExpResult));
        }
        protected virtual IObservable<VictoryOrDefeat> _activateBeforeResultUI(VictoryOrDefeat result)
        {
            var pickUpCharacter = SceneContainer.GetBattleManager().GetPickUpCharacter();
            var characterView = pickUpCharacter != null ? pickUpCharacter.GetView() : null;
            return _uiController.ActivateResultCharacterMessageUIView(characterView, "HOGEHOGE", result == VictoryOrDefeat.Win).Select(_ => result);
        }

        public IObservable<IManager> LoadDatas()
        {
            // キャラクターパネル
            var ownCharacters = SceneContainer.GetCharacterManager().GetCurrentOwnCharacters();
            _ownCharacterPanelUISet = new OwnCharacterPanelUISet(ownCharacters);

            return Observable.Return<IManager>((IManager)this);
        }

        public virtual IObservable<IManager> LoadAssets(IFactory factory)
        {
            return SceneContainer.GetFactory().BuildUIController()
            .SelectMany(uiController =>
            {
                _uiController = uiController;
                return Observable.WhenAll(
                    _ownCharacterPanelUISet.LoadAssets(uiController).Select(_ => this),
                    _loadDropItems(uiController).Select(_ => this),
                    _characterAboveUISet.LoadAssets(factory).Select(_ => this),
                    _loadModalUI(GetPauseModalUI()).Do(modal => modal.SetViewActive(false)).Select(_ => this),
                    _loadModalUI(GetRetryModalUI()).Do(modal => modal.SetViewActive(false)).Select(_ => this)
                );
            })
            .Select(_ => (IManager)this);
        }
        public void Prepare()
        {
            var battleManager = SceneContainer.GetBattleManager();
            var characterManager = SceneContainer.GetCharacterManager();

            // UI 表示初期化
            _setWaveNum(battleManager.WaveNum);
            _setNextWaveButtonVisible(false);

            // アクティブアクション発動処理紐付け
            _ownCharacterPanelUISet.GetOnClickObservable()
            .Subscribe(no => characterManager.CurrentOwnCharacterSet.GetCharacterByNo(no).TryToTransitToActiveAction()).AddTo(_disposables);

            // ドロップ時の更新
            battleManager.DropHandler.GetOnCommandDropItemObservable()
            .Subscribe(die => _onCommandDropItem(die)).AddTo(_disposables);
            battleManager.DropHandler.GetOnCommandDropMoneyObservable()
            .Subscribe(dme => _onCommandDropMoney(dme)).AddTo(_disposables);
            // ポーズボタン処理紐付け
            _uiController.GetOnClickPauseButtonObservable()
            .SelectMany(_ => _pauseModalUI.Open())
            .Subscribe(_ => _onPauseSubject.OnNext(true)).AddTo(_disposables);
            // ポーズモーダルのクローズ処理紐付け
            _pauseModalUI.GetOnCloseEnterObservable()
            .Subscribe(_ => _onPauseSubject.OnNext(false)).AddTo(_disposables);
            // ポーズモーダル撤退ボタンを押した時の処理を紐付け
            _pauseModalUI.GetOnClickWithdrawalObservable()
            .Subscribe(_ => _onTransitScene.OnNext(false)).AddTo(_disposables);
            // 通信リトライモーダルリトライボタン処理紐付け
            _retryModalUI.GetOnCloseEnterObservable()
            .SelectMany(_ => battleManager.SendResult())
            .Subscribe().AddTo(_disposables);
            // オートモード切り替えボタン処理紐付け
            if (battleManager.BattleAutoMode == BattleAutoMode.Selectable)
            {
                _uiController.GetOnToggleAutoModeObservable()
                .Subscribe(_onToggleAutoMode).AddTo(_disposables);
            }
            // Wave切り替えボタンを押した時の動作を紐付け
            _uiController.GetOnClickNextWaveButtonObservable()
            .Subscribe(_ => battleManager.TryToTransitToNextWave()).AddTo(_disposables);
            // 分身キャラクター上部 UI 処理紐付け
            characterManager.GetOnCompleteAddingShadowObservable()
            .SelectMany(c => _characterAboveUISet.LoadCharacterAboveUIView(_uiController, c))
            .Subscribe().AddTo(_disposables);
            characterManager.GetOnRemoveShadowObservable()
            .Subscribe(c => _characterAboveUISet.RemoveCharacterAboveUIView(c)).AddTo(_disposables);

            // レイドボス
            if (battleManager.AdditionalInformation != null && battleManager.AdditionalInformation.WaveGaugeType.HasValue)
            {
                var additionalInfo = battleManager.AdditionalInformation;
                _uiController.InitializeWaveGaugeView(additionalInfo.WaveGaugeLevel, additionalInfo.WaveGaugeTextKey,
                                                          characterManager.GetEnemyCharacterSetTotalCurrentHP(),
                                                          characterManager.GetEnemyCharacterSetTotalMaxHP(),
                                                          additionalInfo.WaveGaugeType.Value);
                characterManager.GetOnEnemyCharacterSetHPChangeObservable()
                .Subscribe(cve => _uiController.ChangeWaveGauge((float)cve.Current / cve.Max)).AddTo(_disposables);
            }

        }
        void _onToggleAutoMode(bool isOn)
        {
            SceneContainer.GetBattleManager().SetAutoMode(isOn);
            SceneContainer.GetCharacterManager().CurrentOwnCharacterSet.SetAuto(isOn);
        }

        IObservable<DropItemUI[]> _loadDropItems(IUIController uiController)
        {
            var dropHandler = SceneContainer.GetBattleManager().DropHandler;
            return Observable.WhenAll(dropHandler.GetDropItems().Select(i =>
            {
                var dropItemUI = new DropItemUI(i.ItemID, i.No);
                _dropItemUIs.Add(dropItemUI.No, dropItemUI);
                return dropItemUI.LoadView(uiController);
            }));
        }

        void _onCommandDropItem(Drop.DropItemEvent dropItemEvent)
        {
            var dropHandler = SceneContainer.GetBattleManager().DropHandler;
            switch (dropItemEvent.Command)
            {
                case Drop.DropCommand.Drop:
                    _dropItemUIs[dropItemEvent.No].Drop(dropItemEvent.CharacterView)
                    .Subscribe(ui => dropHandler.PickUpDropItem(ui.No))
                    .AddTo(_disposables);
                    break;
                case Drop.DropCommand.PickUp:
                    _dropItemUIs[dropItemEvent.No].Gain()
                    .Subscribe(_ => _uiController.SetCurrentItemNum(dropItemEvent.CurrentTotalNumFunc()))
                    .AddTo(_disposables);
                    break;
                default:
                    break;
            }
        }
        void _onCommandDropMoney(Drop.DropMoneyEvent dropMoneyEvent)
        {
            _uiController.SetCurrentMoneyNum(dropMoneyEvent.CurrentTotalMoney);
        }

        void _setCurrentWaveNum(int num)
        {
            _uiController.SetCurrentWaveNum(num);
        }
        void _setWaveNum(int num)
        {
            _uiController.SetMaxWaveNum(num);
        }

        void _setNextWaveButtonVisible(bool visible)
        {
            _uiController.SetNextWaveButtonVisible(visible);
        }

        /// シーン遷移処理がプッシュされる Observable を取得。string は使わないように後で治す。
        public IObservable<bool> GetOnTransitSceneObservable()
        {
            return _onTransitScene.AsObservable();
        }
        public IObservable<bool> GetOnTogglePauseObservable()
        {
            return _onPauseSubject.AsObservable();
        }
        public IObservable<SoundEvent> GetOnCommandSoundObservable()
        {
            return _onCommandSoundSubject.AsObservable();
        }

        void _updateTime(byte time)
        {
            _uiController.UpdateTime(time);
        }

        void _playWinMessage()
        {
            if (SceneContainer.GetBattleManager().BattleResult.GetVictoryOrDefeat() == VictoryOrDefeat.Win) _uiController.PlayWinMessage();
        }

        public PauseModalUI GetPauseModalUI() { return _pauseModalUI; }

        public ModalUIViewModel GetRetryModalUI() { return _retryModalUI; }

        public ResultUI GetWinModalUI() { return _winUI; }
        public IObservable<ResultUI> LoadWinModalUI()
        {
            return _loadResultUI(GetWinModalUI())
            .Select(_ => GetWinModalUI());
        }
        public ResultUI GetLossModalUI() { return _lossUI; }
        public IObservable<ResultUI> LoadLossModalUI()
        {
            return _loadResultUI(GetLossModalUI())
            .Select(_ => GetLossModalUI());
        }

        IObservable<PauseModalUI> _loadModalUI(PauseModalUI pauseModalUI)
        {
            return pauseModalUI.LoadView().Do(v =>
            {
                _uiController.AddModalUIView(v);
            })
            .Select(_ => pauseModalUI);
        }

        IObservable<ModalUIViewModel> _loadModalUI(ModalUIViewModel retryModalUI)
        {
            return retryModalUI.LoadView().Do(v => 
            {
                _uiController.AddModalUIView(v);
            })
            .Select(_ => retryModalUI);
        }

        IObservable<PlayerLevelUpModalUI> _loadModalUI(PlayerLevelUpModalUI modalUI)
        {
            return modalUI.LoadView().Do(v =>
            {
                _uiController.AddModalUIView(v);
            })
            .Select(_ => modalUI);
        }
        IObservable<IResultUIView> _loadResultUI(ResultUI resultUI)
        {
            var battleManager = SceneContainer.GetBattleManager();
            var characterManager = SceneContainer.GetCharacterManager();
            var ownThumbnails = characterManager.CurrentOwnCharacterSet.GetCharacterThumbnails();
            var enemyThumbnails = characterManager.CurrentEnemyCharacterSet.GetCharacterThumbnails();
            var ownDamages = characterManager.CurrentOwnCharacterSet.GetDamages().ToArray();
            var enemyDamages = characterManager.CurrentEnemyCharacterSet.GetDamages().ToArray();

            return resultUI.LoadView(battleManager.BattleResult.GetRank(), battleManager.PlayerExp, battleManager.DropHandler.GetDropMoney(), battleManager.DropHandler.GetDropItemIDs(), ownThumbnails, enemyThumbnails, ownDamages, enemyDamages).Do(v =>
            {
                _uiController.AddResultUIView(v);
            });
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _ownCharacterPanelUISet.Dispose();
        }
    }
}
