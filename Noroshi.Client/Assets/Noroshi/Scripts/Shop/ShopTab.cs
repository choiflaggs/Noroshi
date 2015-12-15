using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Noroshi.UI {
    public class ShopTab : BtnCommon {
        [SerializeField] Text txtShopName;
        [SerializeField] Image imgFrame;
        [SerializeField] Sprite spriteFrameOn;
        [SerializeField] GameObject charaImg;
        [SerializeField] GameObject iconMedal;
        [SerializeField] GameObject iconLock;
        [SerializeField] GameObject iconNew;
        [SerializeField] GameObject iconSale;
        [SerializeField] Text txtRemainTime;
        [SerializeField] GameObject remainTimeWrapper;

        private bool isActive;

        public void SetShopBtn(string name, bool isOpen, bool isNew = false, int remainTime = 0) {
            txtShopName.text = name;
            if(!isOpen) {
                SetEnable(false);
                return;
            }
            SetEnable(true);
            imgFrame.sprite = spriteFrameOn;
            iconLock.SetActive(false);
            iconMedal.SetActive(true);
            if(charaImg != null) {
                charaImg.SetActive(true);
            }
            if(isNew) {
                iconNew.SetActive(true);
            }
            if(remainTime > 0) {
                txtRemainTime.text = remainTime.ToString();
                iconSale.SetActive(true);
            }
        }

        public void SetShopTab(string name, bool isOpen, int remainTime = 0) {
            txtShopName.text = name;
            txtShopName.gameObject.SetActive(true);
            if(isOpen) {
                SetEnable(true);
                txtShopName.color = Constant.TEXT_COLOR_NORMAL_WHITE;
            } else {
                SetEnable(false);
                txtShopName.color = new Color(0.5f, 0.5f, 0.5f);
            }
            if(remainTimeWrapper != null) {
                if(remainTime > 0) {
                    txtRemainTime.text = remainTime.ToString();
                    remainTimeWrapper.SetActive(true);
                } else {
                    remainTimeWrapper.SetActive(false);
                }
            }
        }
    }
}
