using UnityEngine;
using System.Collections;
using UniRx;
using DG.Tweening;

namespace Noroshi.UI {
    public class ShopDetail : MonoBehaviour {
        [SerializeField] GameObject[] detailList;
        [SerializeField] GameObject[] panelContainerList;
        [SerializeField] GameObject[] characterList;
        [SerializeField] BtnCommon btnBack;
        [SerializeField] GameObject detailBg;
        [SerializeField] GameObject detailTop;
        [SerializeField] GameObject detailBottom;
        [SerializeField] GameObject detailTopContent;
        [SerializeField] GameObject btnBackToMain;

        private void Start() {
            btnBack.OnClickedBtn.Subscribe(_ => {
                CloseDetail();
            });
            btnBack.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
            gameObject.SetActive(false);
        }

        private void CloseDetail() {
            TweenA.Add(gameObject, 0.2f, 0).Then(() => {
                gameObject.SetActive(false);
            });
            btnBackToMain.SetActive(true);
        }
        
        public void OpenDetail(int id) {
            var type = 0;
            if(id > 6) {
                type = 2;
            } else if(id > 3) {
                type = 1;
            }
            gameObject.SetActive(true);
            btnBackToMain.SetActive(false);
            for(int i = 0, l = detailList.Length; i < l; i++) {
                if(i == type) {
                    detailList[i].SetActive(true);
                    panelContainerList[i].GetComponent<CanvasGroup>().alpha = 0;
                    TweenA.Add(panelContainerList[i], 0.05f, 1).Delay(0.8f);
                    characterList[i].transform.localPosition = new Vector2(
                        -775, characterList[i].transform.localPosition.y
                    );
                    TweenX.Add(characterList[i], 0.15f, -425).Delay(0.4f);
                } else {
                    detailList[i].SetActive(false);
                }
            }
            TweenA.Add(gameObject, 0.05f, 1);
            detailTop.transform.localPosition = new Vector2(0, 426);
            detailBottom.transform.localPosition = new Vector2(0, -426);
            detailTop.transform.DOLocalMoveY(246, 0.4f).SetEase(Ease.OutCubic).OnUpdate(() => {
                detailTop.SetActive(false);
                detailTop.SetActive(true);
            });
            TweenY.Add(detailBottom, 0.4f, -294).EaseOutCubic();
            TweenA.Add(detailBg, 0.8f, 1).From(0);
        }
    }
}
