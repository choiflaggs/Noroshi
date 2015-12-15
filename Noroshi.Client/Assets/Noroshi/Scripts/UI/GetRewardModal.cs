using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GetRewardModal : MonoBehaviour {
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon btnOverlay;
        [SerializeField] GameObject[] getRewardList;
        [SerializeField] Image[] imgGetRewardList;
        [SerializeField] Text[] txtGetRewardList;

        private void Start() {
            btnClose.OnClickedBtn.Subscribe(_ => {
                CloseModal();
            });
            btnClose.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnOverlay.OnClickedBtn.Subscribe(_ => {
                CloseModal();
            });
            btnOverlay.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        public void OpenModal(Noroshi.Core.WebApi.Response.Possession.PossessionObject[] getList) {
            var gclm = GlobalContainer.LocalizationManager;
            var getItemLength = getList.Length;
            for(int i = 0, l = getRewardList.Length; i < l; i++) {
                if(i < getItemLength) {
                    imgGetRewardList[i].sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", getList[i].ID)
                    );
                    if(getList[i].Num > 1) {
                        txtGetRewardList[i].text = gclm.GetText(getList[i].Name + ".Name") + "  x" + getList[i].Num;
                    } else {
                        txtGetRewardList[i].text = gclm.GetText(getList[i].Name + ".Name");
                    }
                    getRewardList[i].SetActive(true);
                } else {
                    getRewardList[i].SetActive(false);
                }
            }
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1).From(0);
        }

        public void CloseModal() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
