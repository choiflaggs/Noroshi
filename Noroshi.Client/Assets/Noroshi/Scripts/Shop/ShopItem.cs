using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ShopItem : BtnCommon {
        public class ShopItemData {
            public int ShopID;
            public Noroshi.Core.WebApi.Response.Shop.Merchandise Merchandise;
        }

        [SerializeField] Image imgItem;
        [SerializeField] Image imgPriceIcon;
        [SerializeField] Text txtItemName;
        [SerializeField] Text txtItemPrice;
        [SerializeField] Text txtBuyNum;
        [SerializeField] GameObject soldout;

        public Subject<ShopItemData> OnItemClicked = new Subject<ShopItemData>();

        private ShopItemData itemData = new ShopItemData();

        public void SetItemInfo(Noroshi.Core.WebApi.Response.Shop.Merchandise merchandise) {
            imgItem.sprite = Resources.Load<Sprite>("Item/" + merchandise.MerchandisePossessionObject.ID);
            imgPriceIcon.sprite = Resources.Load<Sprite>("Item/Currency/" + merchandise.PaymentPossessionObject.ID);
            txtItemName.text = GlobalContainer.LocalizationManager.GetText(merchandise.MerchandisePossessionObject.Name + ".Name");
            txtItemPrice.text = merchandise.PaymentPossessionObject.Num.ToString();
            txtBuyNum.text = merchandise.MerchandisePossessionObject.Num.ToString();
            if(merchandise.PaymentPossessionObject.Num > merchandise.PaymentPossessionObject.PossessingNum) {
                TweenC.Add(txtItemPrice.gameObject, 0.01f, Constant.TEXT_COLOR_NEGATIVE);
            } else {
                TweenC.Add(txtItemPrice.gameObject, 0.01f, Constant.TEXT_COLOR_NORMAL_DARK);
            }
            if(merchandise.HasAlreadyBought) {
                ShowSoldOut();
            }
            itemData.Merchandise = merchandise;
        }

        public uint GetMerchandiseID() {
            return itemData.Merchandise.ID;
        }

        public void ShowSoldOut() {
            base.SetEnable(false);
            soldout.SetActive(true);
        }

        public override void OnPointerClick(PointerEventData ped) {
            if(!isEnable) {return;}
            base.OnPointerClick(ped);
            OnItemClicked.OnNext(itemData);
        }
    }
}
