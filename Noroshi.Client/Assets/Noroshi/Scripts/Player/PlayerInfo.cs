using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Player;
using Noroshi.Core.Game.GameContent;
using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.UI {
    public class PlayerInfo : MonoBehaviour {
        [SerializeField] Text txtGold;
        [SerializeField] Text txtGem;
        [SerializeField] Text txtStamina;
        [SerializeField] Text txtMaxStamina;
        [SerializeField] Text txtBP;
        [SerializeField] Text txtMaxBP;
        [SerializeField] BtnCommon btnChangeGold;
        [SerializeField] BtnCommon btnBuyGem;
        [SerializeField] BtnCommon btnRecoverStamina;
        [SerializeField] BtnCommon btnRecoverBP;
        [SerializeField] GameObject overlay;
        [SerializeField] PlayerHeaderModal buyGemContainer;
        [SerializeField] GoldChangeModal goldChangeModal;
        [SerializeField] StaminaRecoverModal staminaRecoverModal;
        [SerializeField] BPRecoverModal bpRecoverModal;
        [SerializeField] PlayerLevelUpPanel playerLevelUpPanel;
        [SerializeField] Text txtTeamLevelUp;
        [SerializeField] Text txtStaminaUp;

        public Subject<uint> OnChangedGold = new Subject<uint>();
        public Subject<uint> OnChangedGem = new Subject<uint>();
        public Subject<ushort> OnChangedStamina = new Subject<ushort>();
        public Subject<ushort> OnChangedBP = new Subject<ushort>();
        public Subject<GameContent[]> OnUpPlayerLevel = new Subject<GameContent[]>();
        public Subject<bool> OnQuestComplete = new Subject<bool>();

        public static PlayerInfo Instance;

        public bool isLoad = false;

        private Noroshi.Core.WebApi.Response.PlayerStatus playerStatus;
        private TutorialStep tutorialStep;

        StaminaHandler _staminaHandler = new StaminaHandler();
        ushort _lastStamina;
        uint _lastStaminaUpdatedAt;
        BPHandler _bpHandler = new BPHandler();
        byte _lastBP;
        uint _lastBPUpdatedAt;

        IDisposable staminaTimer;
        IDisposable bpTimer;

        private ushort _playerLevel;
        private uint _haveGold;
        private uint _haveGem;

        public uint PlayerLevel {
            get {return _playerLevel;}
        }

        public uint HaveGold {
            get {return _haveGold;}
            private set {
                _haveGold = value;
                OnChangedGold.OnNext(value);
            }
        }

        public uint HaveGem {
            get {return _haveGem;}
            private set {
                _haveGem = value;
                OnChangedGem.OnNext(value);
            }
        }

        public ushort CurrentStamina {
            get {return _staminaHandler.CurrentValue(_playerLevel, _lastStamina, _lastStaminaUpdatedAt, GlobalContainer.TimeHandler.UnixTime);}
        }

        public ushort MaxStamina {
            get {return _staminaHandler.MaxValue(_playerLevel);}
        }

        public ushort CurrentBP {
            get {return _bpHandler.CurrentValue(_lastBP, _lastBPUpdatedAt, GlobalContainer.TimeHandler.UnixTime);}
        }
        
        public ushort MaxBP {
            get {return _bpHandler.MaxValue();}
        }

        private void Awake() {
            if (Instance == null) {Instance = this;}
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            GlobalContainer.RepositoryManager.PlayerStatusRepository.Get().Do(data => {
                playerStatus = data;
                UpdateStatus();
                var tutorialStepHandler = new TutorialStepHandler(playerStatus.TutorialStep);
                tutorialStep = (TutorialStep)tutorialStepHandler.Step;
                CheckShowTutorial(tutorialStep);
                isLoad = true;
            }).Subscribe();
        }

        private void Start() {
            staminaTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Where(_ => playerStatus != null).Scan(CurrentStamina, (prev, _) => {
                if(prev != CurrentStamina) OnChangedStamina.OnNext(CurrentStamina);
                if(CurrentStamina >= MaxStamina) {
                    staminaTimer.Dispose();
                }
                return CurrentStamina;
            }).Subscribe().AddTo(this);

            bpTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Where(_ => playerStatus != null).Scan(CurrentBP, (prev, _) => {
                if(prev != CurrentBP) OnChangedBP.OnNext(CurrentBP);
                if(CurrentBP >= MaxBP) {
                    bpTimer.Dispose();
                }
                return CurrentBP;
            }).Subscribe().AddTo(this);

            OnChangedGold.Subscribe(n => {
                txtGold.text = string.Format("{0:#,0}\r", HaveGold);
            }).AddTo(this);
            
            OnChangedGem.Subscribe(n => {
                txtGem.text = string.Format("{0:#,0}\r", HaveGem);
            }).AddTo(this);
            
            OnChangedStamina.Subscribe(n => {
                txtStamina.text = CurrentStamina.ToString();
                txtMaxStamina.text = MaxStamina.ToString();
            }).AddTo(this);

            OnChangedBP.Subscribe(n => {
                txtBP.text = CurrentBP.ToString();
                txtMaxBP.text = MaxBP.ToString();
            }).AddTo(this);

            btnChangeGold.OnClickedBtn.Subscribe(_ => {
                goldChangeModal.Open();
            }).AddTo(this);

            btnBuyGem.OnClickedBtn.Subscribe(_ => {
                buyGemContainer.OnOpen(overlay);
            }).AddTo(this);

            btnRecoverStamina.OnClickedBtn.Subscribe(_ => {
                staminaRecoverModal.Open();
            }).AddTo(this);

            btnRecoverBP.OnClickedBtn.Subscribe(_ => {
                bpRecoverModal.Open();
            }).AddTo(this);

            playerLevelUpPanel.Init();
        }

        private void UpdateStatus() {
            _playerLevel = playerStatus.Level;
            HaveGold = playerStatus.Gold;
            HaveGem = playerStatus.Gem;
            _lastStamina = playerStatus.LastStamina;
            _lastStaminaUpdatedAt = playerStatus.LastStaminaUpdatedAt;
            _lastBP = (byte)playerStatus.LastBP;
            _lastBPUpdatedAt = playerStatus.LastBPUpdatedAt;
            OnChangedStamina.OnNext(CurrentStamina);
            OnChangedBP.OnNext(CurrentBP);
        }

        private void CheckShowTutorial(TutorialStep step) {
            if(step == TutorialStep.ClearStoryStage1 ||
                (step >= TutorialStep.ClearStoryStage2 && step < TutorialStep.EquipGear) ||
                (step >= TutorialStep.ClearStoryStage3 && step < TutorialStep.ActionLevelUP) ||
                (step >= TutorialStep.ClearStoryStage4 && step < TutorialStep.PromotionLevelUP) ||
                (step == TutorialStep.ClearStoryStage5) ||
                (step >= TutorialStep.ClearStoryStage6 && step < TutorialStep.ConsumeDrug) ||
                (step >= TutorialStep.ClearStoryStage7 && step < TutorialStep.LotGacha) ||
//                (step >= TutorialStep.ClearStoryStage8 && step < TutorialStep.ReceiveQuestReward) ||
                (step >= TutorialStep.ClearStoryStage9 && step < TutorialStep.ReceiveDailyQuestReward) ||
                (step >= TutorialStep.ClearStoryStage10 && step < TutorialStep.EvolutionLevelUP)
            ) {
                var tutorialCanvas = Instantiate(Resources.Load("Tutorial/TutorialCanvas")) as GameObject;
                tutorialCanvas.GetComponent<TutorialController>().Init(step);
            }
        }

        private void OnApplicationPause(bool isPause) {
            if (isPause) {
                //ホームボタンを押してアプリがバックグランドに移行した時
    //            Debug.Log("バックグランドに移行したよ");
            } else {
                //アプリを終了しないでホーム画面からアプリを起動して復帰した時
    //            Debug.Log("バックグランドから復帰したよ");
            }
        }

        public void ChangeHaveGold(int value) {
            var tempGold = (int)_haveGold + value;
            HaveGold = (uint)tempGold;
        }

        public void ChangeHaveGem(int value) {
            var tempGem = (int)_haveGem + value;
            HaveGem = (uint)tempGem;
        }

        public Noroshi.Core.WebApi.Response.PlayerStatus GetPlayerStatus() {
            return playerStatus;
        }

        public void UpdatePlayerStatus(Noroshi.Core.WebApi.Response.PlayerStatus status = null) {
            if(status == null) {
                GlobalContainer.RepositoryManager.PlayerStatusRepository.Get().Do(data => {
                    playerStatus = data;
                    UpdateStatus();
                }).Subscribe();
            } else {
                playerStatus = status;
                UpdateStatus();
            }
        }

        public TutorialStep GetTutorialStep() {
            return tutorialStep;
        }

        public void ReceivedQuestComplete(AddPlayerExpResult addPlayerExpResult) {
            OnQuestComplete.OnNext(true);
            if(addPlayerExpResult.LevelUp) {
                _playerLevel = addPlayerExpResult.CurrentPlayerLevel;
                playerLevelUpPanel.ShowPanel(addPlayerExpResult);
                var openGameContents = Core.Game.GameContent.GameContent.BuildMulti(addPlayerExpResult.OpenGameContentIDs).ToArray();
                OnUpPlayerLevel.OnNext(openGameContents);
            }
        }
    }
}
