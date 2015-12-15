using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class ShopPanel : MonoBehaviour {
        [SerializeField] Text txtShopTitle;
        [SerializeField] Text txtDescription;
        [SerializeField] Text txtUpdate;
        [SerializeField] Text txtCurrency;
        [SerializeField] BtnCommon btnUpdate;
        [SerializeField] GameObject itemWrapper;
        [SerializeField] ShopItem[] itemList;

        public Subject<ShopItem.ShopItemData> OnBuyConfirm = new Subject<ShopItem.ShopItemData>();
        public Subject<Noroshi.Core.WebApi.Response.Shop.Shop> OnUpdateConfirm = new Subject<Noroshi.Core.WebApi.Response.Shop.Shop>();

        private Noroshi.Core.WebApi.Response.Shop.Shop shopData;

        private void Start() {
            btnUpdate.OnClickedBtn.Subscribe(_ => {
                OnUpdateConfirm.OnNext(shopData);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
            foreach(var item in itemList) {
                item.OnItemClicked.Subscribe(data => {
                    OnBuyConfirm.OnNext(data);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }
        }

        public void SetShopPanel(Noroshi.Core.WebApi.Response.Shop.Shop _shopData) {
            shopData = _shopData;
            txtShopTitle.text = GlobalContainer.LocalizationManager.GetText(shopData.TextKey + ".Name");
            txtDescription.text = GlobalContainer.LocalizationManager.GetText(shopData.TextKey + ".Description");
            if(shopData.NextMerchandiseScheduledUpdateTime != null) {
                string hour = shopData.NextMerchandiseScheduledUpdateTime.Value.Hour.ToString();
                string minute = shopData.NextMerchandiseScheduledUpdateTime.Value.Minute < 10 ?
                    "0" + shopData.NextMerchandiseScheduledUpdateTime.Value.Minute.ToString() :
                        shopData.NextMerchandiseScheduledUpdateTime.Value.Minute.ToString();
                txtUpdate.text = hour + ":" + minute;
            } else {
                txtUpdate.text = "";
            }
            if(txtCurrency != null) {
                txtCurrency.text = shopData.Merchandises[0].PaymentPossessionObject.PossessingNum.ToString();
                txtCurrency.gameObject.SetActive(true);
            }
            for(int i = 0, l = itemList.Length; i < l; i++) {
                if(i < shopData.Merchandises.Length) {
                    itemList[i].SetItemInfo(shopData.Merchandises[i]);
                    itemList[i].gameObject.SetActive(true);
                } else {
                    itemList[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateShopPanel(Noroshi.Core.WebApi.Response.Shop.Shop _shopData) {
            SetShopPanel(_shopData);
        }
    }
}
