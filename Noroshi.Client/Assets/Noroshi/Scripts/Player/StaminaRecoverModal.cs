using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class StaminaRecoverModal : MonoBehaviour {
        [SerializeField] Text txtNeedGem;
        [SerializeField] Text txtCurrentStamina;
        [SerializeField] Text txtNewStamina;
        [SerializeField] Text txtMaxStamina;
        [SerializeField] Text txtRecoverTimes;
        [SerializeField] GameObject iconGem;
        [SerializeField] GameObject enoughWrapper;
        [SerializeField] GameObject notEnoughWrapper;
        [SerializeField] GameObject notEnoughGemAlert;
        [SerializeField] GameObject notRecoverAnyMoreAlert;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] BtnCommon btnCancel;
        [SerializeField] BtnCommon btnClose;

        private Noroshi.Core.Game.Player.RepeatablePaymentCalculator repeatablePaymentCalculator;
        private Noroshi.Player.WebAPIRequester webAPIRequester;

        private void Start() {
            webAPIRequester = new Noroshi.Player.WebAPIRequester();

            btnOK.OnClickedBtn.Subscribe(_ => {
                RecoverStamina();
            });
            btnOK.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.STATUS_UP);
            });

            btnCancel.OnClickedBtn.Subscribe(_ => {
                Close();
            });
            btnCancel.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                Close();
            });
            btnClose.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        private void RecoverStamina() {
            webAPIRequester.RecoverStamina().Do(data => {
                PlayerInfo.Instance.UpdatePlayerStatus(data.PlayerStatus);
                Close();
            }).Subscribe();
        }

        private void Close() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void Open() {
            if(repeatablePaymentCalculator == null) {
                repeatablePaymentCalculator = new Noroshi.Core.Game.Player.RepeatablePaymentCalculator(
                    Noroshi.Core.Game.Player.Constant.RECOVER_GEM_POINT
                );
            }
            var playerStatus = PlayerInfo.Instance.GetPlayerStatus();
            var recoverNum = (ushort)playerStatus.LastStaminaRecoveryNum;
            var needGem = repeatablePaymentCalculator.GetPaymentNum(recoverNum);
            if(PlayerInfo.Instance.CurrentStamina >= PlayerInfo.Instance.MaxStamina) {
                iconGem.SetActive(false);
                txtNeedGem.gameObject.SetActive(false);
                notEnoughGemAlert.SetActive(false);
                notRecoverAnyMoreAlert.SetActive(true);
                enoughWrapper.SetActive(false);
                notEnoughWrapper.SetActive(true);
            } else if(needGem > playerStatus.Gem) {
                iconGem.SetActive(true);
                txtNeedGem.gameObject.SetActive(true);
                txtNeedGem.text = needGem.ToString();
                notEnoughGemAlert.SetActive(true);
                notRecoverAnyMoreAlert.SetActive(false);
                enoughWrapper.SetActive(false);
                notEnoughWrapper.SetActive(true);
            } else {
                iconGem.SetActive(true);
                txtNeedGem.gameObject.SetActive(true);
                txtNeedGem.text = needGem.ToString();
                txtCurrentStamina.text = PlayerInfo.Instance.CurrentStamina.ToString();
                txtNewStamina.text = (PlayerInfo.Instance.CurrentStamina + PlayerInfo.Instance.MaxStamina).ToString();
                txtMaxStamina.text = PlayerInfo.Instance.MaxStamina.ToString();
                txtRecoverTimes.text = (recoverNum + 1).ToString();
                enoughWrapper.SetActive(true);
                notEnoughWrapper.SetActive(false);
            }

            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1);
        }
    }
}
