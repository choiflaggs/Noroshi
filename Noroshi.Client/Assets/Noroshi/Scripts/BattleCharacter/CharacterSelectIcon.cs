using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UniRx;
using Noroshi.Game;

namespace Noroshi.UI {
    public class CharacterSelectIcon : BtnCommon {
        [SerializeField] Text txtInfo;
        [SerializeField] Text txtInfoHeading;
        [SerializeField] GameObject iconSelected;
        [SerializeField] GameObject iconRequire;
        [SerializeField] GameObject iconCPU;
        [SerializeField] GameObject iconBig;
        [SerializeField] Image imgCharacterFrame;
        [SerializeField] Sprite[] imgFrameList;
        [SerializeField] GameObject[] evolutionStar;
        [SerializeField] GameObject continuingStatusContainer;
        [SerializeField] GameObject hpBar;
        [SerializeField] GameObject spBar;

        public Subject<uint> OnClickedIcon = new Subject<uint>();
        public Subject<bool> OnShowRequire = new Subject<bool>();

        public uint playerCharacterID;
        public int position;
        public int lv;
        public int hp;
        public int power;
        public bool isDeca;

        private bool isActive = true;
        private bool isRequire = false;

        public void SetInfo(CharacterStatus status, uint pcID) {
            var img = GetComponent<Image>();

            img.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_{1}", status.CharacterID, status.SkinLevel)
            );
            id = (int)status.CharacterID;
            playerCharacterID = pcID;
            position = (int)status.Position;
            lv = status.Level;
            isDeca = status.TagSet.IsDeca;
            hp = (int)status.MaxHP;
            power = (int)(status.PhysicalAttack + status.MagicPower + status.PhysicalCrit + status.MagicCrit);
            if(isDeca) {iconBig.SetActive(true);}

            imgCharacterFrame.sprite = imgFrameList[status.PromotionLevel - 1];
            SetInfoTxt(0);
            for(int i = 0; i < status.EvolutionLevel; i++) {
                evolutionStar[i].SetActive(true);
            }
            continuingStatusContainer.SetActive(false);
        }

        public void SetInfoTxt(int type) {
            var gclm = GlobalContainer.LocalizationManager;
            switch(type) {
                case 0:
                    txtInfo.text = lv.ToString();
                    txtInfoHeading.text = gclm.GetText("UI.Heading.Level");
                    break;
                case 1:
                    txtInfo.text = hp.ToString();
                    txtInfoHeading.text = gclm.GetText("UI.Heading.HP");
                    break;
                case 2:
                    txtInfo.text = power.ToString();
                    txtInfoHeading.text = gclm.GetText("UI.Heading.Power");
                    break;
                default:
                    break;
            }
        }

        public void SetCPU(uint characterID) {
            var img = GetComponent<Image>();
            
            img.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_1", characterID)
            );
            txtInfo.gameObject.SetActive(false);
            txtInfoHeading.gameObject.SetActive(false);
            imgCharacterFrame.gameObject.SetActive(false);
            iconCPU.SetActive(true);
        }

        public void SetRequire() {
            isRequire = true;
            iconRequire.SetActive(true);
        }

        public void SetContinuingStatus(float hpRatio, float spRatio) {
            if(hpRatio == 0) {
                isActive = false;
                TweenC.Add(gameObject, 0.01f, new Color(0.3f, 0.3f, 0.3f));
            } else {
                isActive = true;
                TweenC.Add(gameObject, 0.01f, new Color(1, 1, 1));
            }
            TweenSX.Add(hpBar, 0.01f, hpRatio);
            TweenSX.Add(spBar, 0.01f, spRatio);
            continuingStatusContainer.SetActive(true);
        }

        public void OnSelect(bool isSelect) {
            if(isSelect) {
                iconSelected.SetActive(true);
            } else {
                iconSelected.SetActive(false);
                iconRequire.SetActive(false);
                isRequire = false;
            }
        }

        public override void OnPointerUp(PointerEventData ped) {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public override void OnPointerClick(PointerEventData ped) {
            if(!isActive) {return;}
            if(isRequire) {
                OnShowRequire.OnNext(true);
                return;
            }
            isActive = false;
            OnClickedIcon.OnNext(playerCharacterID);
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
                isActive = true;
            });
        }
    }
}
