using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Noroshi.UI {
    public class BtnStage : BtnCommon {
        [SerializeField] Image imgStage;
        [SerializeField] Sprite imgCleared;
        [SerializeField] Sprite imgClearedTap;
        [SerializeField] Sprite imgNew;
        [SerializeField] Sprite imgNewTap;
        [SerializeField] GameObject[] starList;

        private Sprite originImg;
        private Sprite tapImg;

        public override void OnPointerDown(PointerEventData ped) {
            if(!isEnable) {return;}
            base.OnPointerDown(ped);
            imgStage.sprite = tapImg;
        }
        
        public override void OnPointerUp(PointerEventData ped) {
            if(!isEnable) {return;}
            base.OnPointerUp(ped);
            imgStage.sprite = originImg;
        }

        public void SetButtonState(bool isClear, bool isMainStage, bool isNew, byte rank = 0) {
            bool isEnable = false;
            if((isClear && isMainStage) || isNew) {isEnable = true;}
            base.SetEnable(isEnable);

            if(isClear) {
                if(isMainStage) {
                    imgStage.sprite = imgCleared;
                    originImg = imgCleared;
                    tapImg = imgClearedTap;
                    for(int i = 0, l = starList.Length; i < l; i++) {
                        if(i < rank) {
                            starList[i].SetActive(true);
                        }
                    }
                } else {
                    imgStage.sprite = imgCleared;
                    imgStage.transform.localScale = Vector3.one;
                }
            } else if(isNew) {
                imgStage.sprite = imgNew;
                originImg = imgNew;
                tapImg = imgNewTap;
                if(!isMainStage) {
                    imgStage.transform.localScale = Vector3.one * 0.8f;
                }
            }
        }
    }
}
