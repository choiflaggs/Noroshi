using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class TreasureInfo : MonoBehaviour {
        [SerializeField] BtnCommon btnCloseTreasureInfo;
        [SerializeField] BtnCommon btnOverlay;
        [SerializeField] Text txtGetCoin;
        [SerializeField] GameObject[] getItemList;
        [SerializeField] Image[] imgGetItemList;
        [SerializeField] Text[] txtGetItemList;

        private void Start() {
            btnCloseTreasureInfo.OnClickedBtn.Subscribe(_ => {
                CloseTreasureInfo();
            });
            btnCloseTreasureInfo.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnOverlay.OnClickedBtn.Subscribe(_ => {
                CloseTreasureInfo();
            });
            btnOverlay.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        public void OpenTreasureInfo(uint getMoney, Noroshi.Core.WebApi.Response.Possession.PossessionObject[] getList) {
            var getItemLength = getList.Length;
            txtGetCoin.text = getMoney.ToString();
            for(int i = 0, l = getItemList.Length; i < l; i++) {
                if(i < getItemLength) {
                    imgGetItemList[i].sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", getList[i].ID)
                    );
                    if(getList[i].Num > 1) {
                        txtGetItemList[i].text = getList[i].Name + "  x" + getList[i].Num;
                    } else {
                        txtGetItemList[i].text = getList[i].Name;
                    }
                    getItemList[i].SetActive(true);
                } else {
                    getItemList[i].SetActive(false);
                }
            }
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.2f, 1).From(0);
        }

        public void CloseTreasureInfo() {
            TweenA.Add(gameObject, 0.2f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
