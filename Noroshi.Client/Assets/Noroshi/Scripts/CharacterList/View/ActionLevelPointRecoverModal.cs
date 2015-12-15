using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ActionLevelPointRecoverModal : MonoBehaviour {
        [SerializeField] Text txtNeedGem;
        [SerializeField] Text txtNewActionLevelPoint;
        [SerializeField] Text txtMaxActionLevelPoint;
        [SerializeField] Text txtRecoverTimes;
        [SerializeField] GameObject enoughWrapper;
        [SerializeField] GameObject notEnoughWrapper;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] BtnCommon btnCancel;
        [SerializeField] BtnCommon btnClose;

        public Subject<bool> OnRecoverActionLevelPoint = new Subject<bool>();

        private Noroshi.Core.Game.Player.RepeatablePaymentCalculator repeatablePaymentCalculator;
        private Noroshi.Player.WebAPIRequester webAPIRequester;

        private void Start() {
            webAPIRequester = new Noroshi.Player.WebAPIRequester();

            btnOK.OnClickedBtn.Subscribe(_ => {
                RecoverActionLevelPoint();
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

        private void RecoverActionLevelPoint() {
            webAPIRequester.RecoverActionLevelPoint().Do(data => {
                PlayerInfo.Instance.UpdatePlayerStatus(data.PlayerStatus);
                OnRecoverActionLevelPoint.OnNext(true);
                Close();
            }).Subscribe();
        }

        private void Close() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void Open(byte maxPoint) {
            if(repeatablePaymentCalculator == null) {
                repeatablePaymentCalculator = new Noroshi.Core.Game.Player.RepeatablePaymentCalculator(
                    Noroshi.Core.Game.Player.Constant.RECOVER_GEM_TO_ACTION_LEVEL_POINT
                );
            }
            var playerStatus = PlayerInfo.Instance.GetPlayerStatus();
            var recoverNum = (ushort)playerStatus.LastActionLevelPointRecoveryNum;
            var needGem = repeatablePaymentCalculator.GetPaymentNum(recoverNum);
            if(needGem > playerStatus.Gem) {
                txtNeedGem.text = needGem.ToString();
                enoughWrapper.SetActive(false);
                notEnoughWrapper.SetActive(true);
            } else {
                txtNeedGem.text = needGem.ToString();
                txtNewActionLevelPoint.text = maxPoint.ToString();
                txtMaxActionLevelPoint.text = maxPoint.ToString();
                txtRecoverTimes.text = (recoverNum + 1).ToString();
                enoughWrapper.SetActive(true);
                notEnoughWrapper.SetActive(false);
            }

            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1);
        }
    }
}
