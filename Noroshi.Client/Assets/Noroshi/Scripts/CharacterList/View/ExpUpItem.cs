using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class ExpUpItem : MonoBehaviour {
        [SerializeField] Text txtHaveNum;
        [SerializeField] Image imgDrug;
        [SerializeField] BtnCommon btnUse;
        [SerializeField] Material grayScale;

        public Subject<int> OnPanelClick = new Subject<int>();

        public int index;

        private int itemID;
        private int haveNum;
        private int useNum = 0;
        private bool isTouch = false;
        private float interval = 0.4f;
        private float crntInterval = 0.0f;

        private void Start() {
            btnUse.OnTouchBtn.Subscribe(_ => {
                isTouch = true;
            });
            btnUse.OnReleaseBtn.Subscribe(_ => {
                isTouch = false;
                interval = 0.4f;
                crntInterval = 0.0f;
            });
        }

        private void Update() {
            if(!isTouch) {return;}
            crntInterval -= Time.deltaTime;
            if(crntInterval < 0) {
                crntInterval = interval;
                if(interval > 0.12f) {interval -= 0.04f;}
                UpdateHaveNum();
            }
        }

        private void UpdateHaveNum() {
            if(haveNum < 1) {return;}
            haveNum--;
            useNum++;
            txtHaveNum.text = haveNum.ToString();
            OnPanelClick.OnNext(index);
            SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
        }

        public void SetExpUpPanel(int id, uint _haveNum) {
            itemID = id;
            imgDrug.sprite = Resources.Load<Sprite>("Item/" + id);
            haveNum = (int)_haveNum;
            txtHaveNum.text = haveNum.ToString();
            SetEnable(true);
        }

        public void SetEnable(bool isEnable) {
            if(isEnable && haveNum > 0) {
                btnUse.SetEnable(true);
                imgDrug.material = null;
            } else {
                btnUse.SetEnable(false);
                imgDrug.material = grayScale;
            }
            isTouch = false;
        }

        public int GetItemID() {
            return itemID;
        }

        public int GetUseNum() {
            return useNum;
        }

        public void ResetUseNum() {
            useNum = 0;
        }
    }
}
