using UnityEngine;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ShopController : MonoBehaviour {
        [SerializeField] ShopTab[] shopBtnList;
        [SerializeField] ShopTab[] shopTabList;
        [SerializeField] ShopPanel[] shopPanelList;
        [SerializeField] GameObject[] shopInfoList;
        [SerializeField] GameObject[] shopDescriptionList;
        [SerializeField] ShopDetail shopDetail;
        [SerializeField] BtnCommon btnToShops;
        [SerializeField] BtnCommon btnToExchange;
        [SerializeField] GameObject background;
        [SerializeField] ShopConfirm shopConfirm;
        [SerializeField] ShopUpdate shopUpdate;

        private bool isLoad = false;
        private float startDragPosition;
        private int currentShopType = 0;
        private int openIndex;

        private void Start() {
            btnToShops.OnClickedBtn.Subscribe(_ => {
                ChangeShopType(true);
            });
            btnToShops.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnToExchange.OnClickedBtn.Subscribe(_ => {
                ChangeShopType(false);
            });
            btnToExchange.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            shopConfirm.OnBuyItem.Subscribe(data => {
                Noroshi.Shop.WebAPIRequester.Buy(data.Merchandise.DisplayID).Do(shopData => {
                    shopPanelList[openIndex].UpdateShopPanel(shopData.Shop);
                    PlayerInfo.Instance.UpdatePlayerStatus();
                    shopConfirm.CloseConfirm();
                }).Subscribe();
            });

            shopUpdate.OnUpdateShop.Subscribe(shopID => {
                Noroshi.Shop.WebAPIRequester.UpdateMerchandises(shopID).Do(shopData => {
                    shopPanelList[openIndex].UpdateShopPanel(shopData.Shop);
                    shopUpdate.CloseUpdate();
                }).Subscribe();
            });

            LoadShopData();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad) {
                yield return new WaitForEndOfFrame();
            }
            OpenDefaultShop();
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void LoadShopData() {
            Noroshi.Shop.WebAPIRequester.List().Do(shopDataList => {
                Debug.Log(shopDataList.Shops.Length);
                for(int i = 0, iz = shopDataList.Shops.Length; i < iz; i++) {
                    var shopData = shopDataList.Shops[i];
                    for(int j = 0, jz = shopBtnList.Length; j < jz; j++) {
                        if(shopData.ID == shopBtnList[j].id) {
                            var n = j;
                            shopBtnList[j].SetShopBtn(GlobalContainer.LocalizationManager.GetText(shopData.TextKey + ".Name"), shopData.IsOpen);
                            shopBtnList[j].OnClickedBtn.Subscribe(id => {
                                SwitchTab(n);
                                Debug.Log(n);
                                shopDetail.OpenDetail(id);
                            });
                            shopBtnList[j].OnPlaySE.Subscribe(_ => {
                                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                            });
                            shopTabList[j].SetShopTab(GlobalContainer.LocalizationManager.GetText(shopData.TextKey + ".Name"), shopData.IsOpen);
                            shopTabList[j].OnClickedBtn.Subscribe(SwitchTab);
                            shopTabList[j].OnPlaySE.Subscribe(_ => {
                                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                            });
                            shopPanelList[j].SetShopPanel(shopData);
                            shopPanelList[j].OnBuyConfirm.Subscribe(data => {
                                shopConfirm.OpenConfirm(data);
                            });
                            shopPanelList[j].OnUpdateConfirm.Subscribe(data => {
                                openIndex = n;
                                shopUpdate.OpenUpdate(data);
                            });
                            break;
                        }
                    }
                }
                isLoad = true;
            }).Subscribe();
        }

        private void ChangeShopType(bool isToShops, float duration = 0.6f) {
            if(isToShops) {
                currentShopType = 1;
                TweenX.Add(background, duration, -Constant.SCREEN_BASE_WIDTH).EaseInOutQuart();
            } else {
                currentShopType = 0;
                TweenX.Add(background, duration, 0).EaseInOutQuart();
            }
        }

        private void SwitchTab(int index) {
            openIndex = index;
            for(int i = 0, l = shopTabList.Length; i < l; i++) {
                var isSelect = i == index;
                shopTabList[i].SetSelect(isSelect);
                shopPanelList[i].gameObject.SetActive(isSelect);
                shopInfoList[i].gameObject.SetActive(isSelect);
                shopDescriptionList[i].gameObject.SetActive(isSelect);
            }
        }

        private void OpenDefaultShop() {
            var prevScene = UILoading.Instance.HistoryList[UILoading.Instance.HistoryList.Count - 1];
            var index = -1;
            if(prevScene == Constant.SCENE_ARENA) {
                index = 3;
            } else if(prevScene == Constant.SCENE_EXPEDITION) {
                index = 3;
            } else if(prevScene == Constant.SCENE_GUILD) {
                index = 5;
            }
            if(index > -1) {
                SwitchTab(index);
                shopDetail.OpenDetail(index);
            }
        }

        public void OnBeginDrag() {
            background.PauseTweens();
            startDragPosition = background.transform.localPosition.x;
        }

        public void OnEndDrag() {
            var diffX = background.transform.localPosition.x - startDragPosition;
            if(currentShopType == 0) {
                ChangeShopType(diffX < -Constant.SCREEN_BASE_WIDTH / 5);
            } else {
                ChangeShopType(diffX < Constant.SCREEN_BASE_WIDTH / 5);
            }
        }
    }
}
