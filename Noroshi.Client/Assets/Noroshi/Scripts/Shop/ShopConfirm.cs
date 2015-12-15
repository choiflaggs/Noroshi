using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ShopConfirm : MonoBehaviour {
        [SerializeField] GameObject confirmPanel;
        [SerializeField] Text txtName;
        [SerializeField] Image imgItem;
        [SerializeField] Text txtDescription;
        [SerializeField] Text txtHaveNum;
        [SerializeField] Text txtBuyNum;
        [SerializeField] Text txtPrice;
        [SerializeField] Image imgCurrency;
        [SerializeField] BtnCommon btnBuy;
        [SerializeField] BtnCommon btnCancel;

        public Subject<ShopItem.ShopItemData> OnBuyItem = new Subject<ShopItem.ShopItemData>();

        private ShopItem.ShopItemData itemData;

        private void Start() {
            btnBuy.OnClickedBtn.Subscribe(_ => {
                OnBuyItem.OnNext(itemData);
            });
            btnBuy.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });

            btnCancel.OnClickedBtn.Subscribe(_ => {
                CloseConfirm();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        public void OpenConfirm(ShopItem.ShopItemData _itemData) {
            var gcrm = GlobalContainer.RepositoryManager;
            gameObject.SetActive(false);
            itemData = _itemData;
            txtName.text = GlobalContainer.LocalizationManager.GetText(itemData.Merchandise.MerchandisePossessionObject.Name + ".Name");
            imgItem.sprite = Resources.Load<Sprite>("Item/" + itemData.Merchandise.MerchandisePossessionObject.ID);
            imgCurrency.sprite = Resources.Load<Sprite>("Item/Currency/" + itemData.Merchandise.PaymentPossessionObject.ID);
            txtDescription.text = GlobalContainer.LocalizationManager.GetText(itemData.Merchandise.MerchandisePossessionObject.Name + ".Description");
            txtHaveNum.text = itemData.Merchandise.MerchandisePossessionObject.PossessingNum.ToString();
            txtBuyNum.text = itemData.Merchandise.MerchandisePossessionObject.Num.ToString();
            txtPrice.text = itemData.Merchandise.PaymentPossessionObject.Num.ToString();
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.2f, 1);

            if(itemData.Merchandise.PaymentPossessionObject.Num > itemData.Merchandise.PaymentPossessionObject.PossessingNum) {
                btnBuy.SetEnable(false);
            } else {
                btnBuy.SetEnable(true);
            }
        }

        public void CloseConfirm() {
            TweenA.Add(gameObject, 0.2f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
