using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ShopUpdate : MonoBehaviour {
        [SerializeField] Text txtPrice;
        [SerializeField] Text txtMaxUpdateNum;
        [SerializeField] Text txtUpdateNum;
        [SerializeField] Image imgCurrency;
        [SerializeField] GameObject enableUpdate;
        [SerializeField] GameObject disableUpdate;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] BtnCommon btnCancel;

        public Subject<uint> OnUpdateShop = new Subject<uint>();

        private uint shopID;

        private void Start() {
            btnOK.OnClickedBtn.Subscribe(_ => {
                OnUpdateShop.OnNext(shopID);
            });
            btnOK.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });

            btnCancel.OnClickedBtn.Subscribe(_ => {
                CloseUpdate();
            });
            btnCancel.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        public void OpenUpdate(Noroshi.Core.WebApi.Response.Shop.Shop shopData) {
            shopID = shopData.ID;
            txtPrice.text = shopData.ManualUpdatePossessionObject.Num.ToString();
            txtMaxUpdateNum.text = shopData.MaxMerchandiseManualUpdateNum.ToString();
            txtUpdateNum.text = shopData.CurrentMerchandiseManualUpdateNum.ToString();
            imgCurrency.sprite = Resources.Load<Sprite>("Item/Currency/" + shopData.ManualUpdatePossessionObject.ID);

            if(shopData.MaxMerchandiseManualUpdateNum > shopData.CurrentMerchandiseManualUpdateNum) {
                enableUpdate.SetActive(true);
                disableUpdate.SetActive(false);
            } else {
                enableUpdate.SetActive(false);
                disableUpdate.SetActive(true);
            }

            if(shopData.ManualUpdatePossessionObject.Num > shopData.ManualUpdatePossessionObject.PossessingNum ||
               shopData.MaxMerchandiseManualUpdateNum <= shopData.CurrentMerchandiseManualUpdateNum) {
                btnOK.SetEnable(false);
            } else {
                btnOK.SetEnable(true);
            }
            
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.2f, 1);
        }
        
        public void CloseUpdate() {
            TweenA.Add(gameObject, 0.2f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
