using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using Noroshi.Core.Game.Player;
using Noroshi.Game;
using Noroshi.UI;
using NoroshiDebug.DebugPlayer;
using NoroshiDebug.TimeDebug;
using NoroshiDebug.Repositories.Server;

namespace Noroshi.NoroshiDebug {
    public class DebugPlayerController : MonoBehaviour {
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtPlayerLevel;
        [SerializeField] Text txtGold;
        [SerializeField] Text txtGem;
        [SerializeField] Text txtStamina;
        [SerializeField] Text txtMaxStamina;
        [SerializeField] Text txtYear;
        [SerializeField] Text txtMonth;
        [SerializeField] Text txtDay;
        [SerializeField] Text txtHour;
        [SerializeField] Text txtMinute;
        [SerializeField] Text txtSecond;
        [SerializeField] Text txtDayOfWeek;
        [SerializeField] BtnCommon btnResetData;
        [SerializeField] BtnCommon btnChangePlayer;
        [SerializeField] BtnCommon btnPlusPlayerLevel;
        [SerializeField] BtnCommon btnMinusPlayerLevel;
        [SerializeField] BtnCommon btnPlus10PlayerLevel;
        [SerializeField] BtnCommon btnMinus10PlayerLevel;
        [SerializeField] BtnCommon btnPlusGold;
        [SerializeField] BtnCommon btnMinusGold;
        [SerializeField] BtnCommon btnPlus10Gold;
        [SerializeField] BtnCommon btnMinus10Gold;
        [SerializeField] BtnCommon btnPlusGem;
        [SerializeField] BtnCommon btnMinusGem;
        [SerializeField] BtnCommon btnPlus10Gem;
        [SerializeField] BtnCommon btnMinus10Gem;
        [SerializeField] BtnCommon btnStamina1;
        [SerializeField] BtnCommon btnStaminaFull;
        [SerializeField] BtnCommon btnTimeSetting;
        [SerializeField] DebugTimeSetPanel timeSetPanel;
        [SerializeField] GameObject processing;
        [SerializeField] GameObject resetConfirmPanel;
        [SerializeField] GameObject changePlayerPanel;
        [SerializeField] BtnCommon btnCancelReset;
        [SerializeField] BtnCommon btnDecideReset;
        [SerializeField] BtnCommon btnCancelChangePlayer;
        [SerializeField] BtnCommon btnDecideChangePlayer;
        [SerializeField] InputField inputChangeID;

        private int currentExp;
        private int currentPlayerLevel;
        private int currentGold;
        private int currentGem;
        private int currentStamina;
        private int maxStamina;

        private PlayerStatusDebugReporitosy _reporitosy = new PlayerStatusDebugReporitosy();
        private TimeDebugRepository _timeDebug = new TimeDebugRepository();
        private bool isLoad = false;
        private bool isSetTime = false;

        StaminaHandler _staminaHandler = new StaminaHandler();

        private void Start() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerStatusRepository.Get().Do(playerData => {
                currentExp = (int)playerData.Exp;
                currentPlayerLevel = (int)playerData.Level;
                currentGold = (int)playerData.Gold;
                currentGem = (int)playerData.Gem;
                currentStamina = (int)_staminaHandler.CurrentValue(playerData.Level, playerData.LastStamina, playerData.LastStaminaUpdatedAt, GlobalContainer.TimeHandler.UnixTime);
                maxStamina = (int)_staminaHandler.MaxValue(playerData.Level);
                txtPlayerName.text = playerData.Name.ToString();
                txtPlayerLevel.text = playerData.Level.ToString();
                txtGold.text = playerData.Gold.ToString();
                txtGem.text = playerData.Gem.ToString();
                txtStamina.text = currentStamina.ToString();
                txtMaxStamina.text = maxStamina.ToString();

                isLoad = true;
            }).Subscribe();

            _timeDebug.Get().Do(timeDebugData => {
                SetDebugTime(timeDebugData.DebugServerTime);
                isSetTime = true;
            }).Subscribe();

            btnResetData.OnClickedBtn.Subscribe(_ => {
                resetConfirmPanel.SetActive(true);
            });
            btnCancelReset.OnClickedBtn.Subscribe(_ => {
                resetConfirmPanel.SetActive(false);
            });
            btnDecideReset.OnClickedBtn.Subscribe(_ => {
                processing.SetActive(true);
                WebApiRequester.Reset().Delay(TimeSpan.FromSeconds(3)).Do(__ => {
                    Destroy(BattleCharacterSelect.Instance.gameObject);
                    UILoading.Instance.ResetHistory();
                    PlayerPrefs.DeleteAll();
                    WebApi.WebApiRequester.ResetSessionID();
                    Application.LoadLevel(Noroshi.UI.Constant.SCENE_LOGIN);
                }).Subscribe();
            });
            btnDecideReset.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.LEVEL_UP);
            });

            btnChangePlayer.OnClickedBtn.Subscribe(_ => {
                changePlayerPanel.SetActive(true);
            });
            btnCancelChangePlayer.OnClickedBtn.Subscribe(_ => {
                changePlayerPanel.SetActive(false);
            });
            btnDecideChangePlayer.OnClickedBtn.Subscribe(_ => {
                uint id = 0;
                var test = uint.TryParse(inputChangeID.text, out id);
                if(id < 1) {return;}
                processing.SetActive(true);
                WebApiRequester.Swap(id).Delay(TimeSpan.FromSeconds(3)).Do(__ => {
                    Destroy(BattleCharacterSelect.Instance.gameObject);
                    UILoading.Instance.ResetHistory();
                    PlayerPrefs.DeleteAll();
                    WebApi.WebApiRequester.ResetSessionID();
                    Application.LoadLevel(Noroshi.UI.Constant.SCENE_LOGIN);
                }).Subscribe();
            });

            btnPlusPlayerLevel.OnClickedBtn.Subscribe(ChangePlayerLevel);
            btnMinusPlayerLevel.OnClickedBtn.Subscribe(ChangePlayerLevel);
            btnPlus10PlayerLevel.OnClickedBtn.Subscribe(ChangePlayerLevel);
            btnMinus10PlayerLevel.OnClickedBtn.Subscribe(ChangePlayerLevel);

            btnPlusGold.OnClickedBtn.Subscribe(ChangeGold);
            btnMinusGold.OnClickedBtn.Subscribe(ChangeGold);
            btnPlus10Gold.OnClickedBtn.Subscribe(ChangeGold);
            btnMinus10Gold.OnClickedBtn.Subscribe(ChangeGold);

            btnPlusGem.OnClickedBtn.Subscribe(ChangeGem);
            btnMinusGem.OnClickedBtn.Subscribe(ChangeGem);
            btnPlus10Gem.OnClickedBtn.Subscribe(ChangeGem);
            btnMinus10Gem.OnClickedBtn.Subscribe(ChangeGem);

            btnStaminaFull.OnClickedBtn.Subscribe(_ => {
                RecoverStamina();
            });
            btnStamina1.OnClickedBtn.Subscribe(ChangeStamina);

            btnTimeSetting.OnClickedBtn.Subscribe(_ => {
                timeSetPanel.OpenTimeSetPanel();
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            timeSetPanel.OnSetDebugTime.Subscribe(debugTime => {
                SetDebugTime(debugTime);
            });

            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad || !isSetTime) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }
        
        private void SetDebugTime(DateTime debugTime) {
            var hour = debugTime.Hour;
            string hourString = hour < 10 ? "0" + hour : hour.ToString();
            var minute = debugTime.Minute;
            string minuteString = minute < 10 ? "0" + minute : minute.ToString();
            var second = debugTime.Second;
            string secondString = second < 10 ? "0" + second : second.ToString();
            txtYear.text = debugTime.Year.ToString();
            txtMonth.text = debugTime.Month.ToString();
            txtDay.text = debugTime.Day.ToString();
            txtHour.text = hourString;
            txtMinute.text = minuteString;
            txtSecond.text = secondString;
            txtDayOfWeek.text = debugTime.DayOfWeek.ToString();
        }
        
        private void ChangePlayerLevel(int value) {
            currentPlayerLevel += value;
            if(currentPlayerLevel > 99) {
                currentPlayerLevel = 99;
            }
            if(currentPlayerLevel < 1) {
                currentPlayerLevel = 1;
            }
            processing.SetActive(true);
            _reporitosy.ChangeLevel((ushort)currentPlayerLevel).Do(playerStatus => {
                currentPlayerLevel = playerStatus.Level;
                currentExp = (int)playerStatus.Exp;
                currentStamina = (int)_staminaHandler.CurrentValue(playerStatus.Level, playerStatus.LastStamina, playerStatus.LastStaminaUpdatedAt, GlobalContainer.TimeHandler.UnixTime);
                maxStamina = (int)_staminaHandler.MaxValue(playerStatus.Level);
                txtPlayerLevel.text = currentPlayerLevel.ToString();
                txtStamina.text = currentStamina.ToString();
                txtMaxStamina.text = maxStamina.ToString();
                processing.SetActive(false);
            }).Subscribe();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void ChangeGold(int value) {
            currentGold += value;
            if(currentGold > 99999999) {
                currentGold = 99999999;
            }
            if(currentGold < 0) {
                currentGold = 0;
            }
            processing.SetActive(true);
            _reporitosy.ChangeGold((uint)currentGold).Do(playerStatus => {
                currentGold = (int)playerStatus.Gold;
                txtGold.text = currentGold.ToString();
                processing.SetActive(false);
            }).Subscribe();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void ChangeGem(int value) {
            currentGem += value;
            if(currentGem > 99999999) {
                currentGem = 99999999;
            }
            if(currentGem < 0) {
                currentGem = 0;
            }
            processing.SetActive(true);
            _reporitosy.ChangeGem((uint)currentGem).Do(playerStatus => {
                currentGem = (int)playerStatus.Gem;
                txtGem.text = currentGem.ToString();
                processing.SetActive(false);
            }).Subscribe();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void RecoverStamina() {
            processing.SetActive(true);
            _reporitosy.ChangeStamina((ushort)maxStamina).Do(playerStatus => {
                currentStamina = (int)_staminaHandler.CurrentValue(playerStatus.Level, playerStatus.LastStamina, playerStatus.LastStaminaUpdatedAt, GlobalContainer.TimeHandler.UnixTime);
                txtStamina.text = currentStamina.ToString();
                processing.SetActive(false);
            }).Subscribe();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void ChangeStamina(int value) {
            currentStamina = value;
            processing.SetActive(true);
            _reporitosy.ChangeStamina((ushort)value).Do(playerStatus => {
                currentStamina = (int)_staminaHandler.CurrentValue(playerStatus.Level, playerStatus.LastStamina, playerStatus.LastStaminaUpdatedAt, GlobalContainer.TimeHandler.UnixTime);
                txtStamina.text = currentStamina.ToString();
                processing.SetActive(false);
            }).Subscribe();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
    }
}
