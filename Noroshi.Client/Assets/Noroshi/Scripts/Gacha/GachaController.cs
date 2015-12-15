using UnityEngine;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GachaController : MonoBehaviour {
        [SerializeField] BtnCommon[] btnDrawGachaList;
        [SerializeField] BtnCommon btnBronze;
        [SerializeField] BtnCommon btnBronzeMulti;
        [SerializeField] BtnCommon btnGold;
        [SerializeField] BtnCommon btnGoldMulti;
        [SerializeField] BtnCommon btnTutorialGacha;
        [SerializeField] GachaResult gachaResult;

        private void Start() {
            Noroshi.Gacha.WebApiRequester.EntryPointList().Do(data => {
                for(int i = 0, l = data.GachaEntryPoints.Length; i < l; i++) {
                    uint id = data.GachaEntryPoints[i].ID;
                    if(data.GachaEntryPoints[i].CanLot) {
                        btnDrawGachaList[i].SetEnable(true);
                        btnDrawGachaList[i].OnClickedBtn.Subscribe(index => {
                            DrawGacha(id, index);
                        });
                        btnDrawGachaList[i].OnPlaySE.Subscribe(_ => {
                            SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
                        });
                    } else {
                        btnDrawGachaList[i].SetEnable(false);
                    }
                }
            }).Subscribe();

            btnTutorialGacha.OnClickedBtn.Subscribe(_ => {
                DrawTutorialGacha();
            });

            gachaResult.OnGachaAgain.Subscribe(type => {
                PlayGacha(type);
            });

            UILoading.Instance.HideLoading();
        }

        private void DrawGacha(uint id, int index) {
            Noroshi.Gacha.WebApiRequester.Lot(id).Do(data => {
                btnDrawGachaList[index].SetEnable(data.GachaEntryPoint.CanLot);
                gachaResult.SetGachaResult(data.LotPossessionObjects);
            }).Subscribe();
        }

        private void DrawTutorialGacha() {
            Noroshi.Gacha.WebApiRequester.LotTutorialGacha().Do(data => {
                btnTutorialGacha.gameObject.SetActive(false);
                btnDrawGachaList[2].gameObject.SetActive(true);
                gachaResult.SetGachaResult(data.LotPossessionObjects);
            }).Subscribe();
        }

        private void PlayGacha(int type) {
            gachaResult.StartAnimation(type);
        }
    }
}
