using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class CharacterListConfirm : MonoBehaviour {
        [SerializeField] Text txtNeedGold;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] BtnCommon btnCancel;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] GameObject confirmText;
        [SerializeField] GameObject alertText;
        [SerializeField] GameObject panel;

        public Subject<CharacterPanel.CharaData> OnDecide = new Subject<CharacterPanel.CharaData>();
        public Subject<bool> OnPlaySE = new Subject<bool>();

        private CharacterPanel.CharaData charaData;
        private uint needGold;

        private void Start() {
            btnOK.OnClickedBtn.Subscribe(_ => {
                OnDecide.OnNext(charaData);
                PlayerInfo.Instance.ChangeHaveGold(-(int)needGold);
                CloseConfirm();
            });
            btnOK.OnPlaySE.Subscribe(_ => {
                OnPlaySE.OnNext(true);
            });

            btnCancel.OnClickedBtn.Subscribe(_ => {
                CloseConfirm();
            });
            btnCancel.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                CloseConfirm();
            });
            btnClose.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        public void OpenConfirm(CharacterPanel.CharaData data, uint value) {
            bool isEnough = PlayerInfo.Instance.HaveGold >= value;
            charaData = data;
            needGold = value;
            txtNeedGold.text = value.ToString();
            btnOK.gameObject.SetActive(isEnough);
            btnCancel.gameObject.SetActive(isEnough);
            btnClose.gameObject.SetActive(!isEnough);
            confirmText.SetActive(isEnough);
            alertText.SetActive(!isEnough);

            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.05f, 0.5f).From(0).EaseOutCubic();
            TweenA.Add(panel, 0.15f, 1).From(0);
        }

        public void CloseConfirm() {
            TweenA.Add(gameObject, 0.15f, 0).EaseInCubic();
            TweenA.Add(panel, 0.15f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
