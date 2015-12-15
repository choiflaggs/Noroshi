using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;
using Noroshi.UI;
using Noroshi.Core.Game.Player;

namespace Noroshi.CharacterList {
    public class BtnEquip : MonoBehaviour, IPointerClickHandler {
        [SerializeField] GameObject iconLvShortage;
        [SerializeField] GameObject iconCanEquip;
        [SerializeField] GameObject iconCanCraft;
        [SerializeField] GameObject iconCanCraftLvShortage;
        [SerializeField] Material grayScale;
        [SerializeField] Image imgGear;
        [SerializeField] Sprite originalImg;

        public Subject<int> OnClickedBtn = new Subject<int>();

        public int index;
        public int equipState;

        public void OnPointerClick(PointerEventData ped) {
            if(PlayerInfo.Instance.GetTutorialStep() >= TutorialStep.ClearStoryStage2) {
                OnClickedBtn.OnNext(index);
            }
        }

        public void SetGearImage(uint gearID, int state) {
            equipState = state;
            iconLvShortage.SetActive(false);
            iconCanEquip.SetActive(false);
            iconCanCraft.SetActive(false);
            iconCanCraftLvShortage.SetActive(false);
            imgGear.sprite = Resources.Load<Sprite>("Item/" + gearID);
            if(state > 0) {
                imgGear.material = null;
            } else {
                imgGear.material = grayScale;
                if(state < -3) {
                    iconCanCraftLvShortage.SetActive(true);
                } else if(state < -2) {
                    iconCanCraft.SetActive(true);
                } else if(state < -1) {
                    iconCanEquip.SetActive(true);
                } else if(state < 0) {
                    iconLvShortage.SetActive(true);
                } else if(state == 0) {
//                    imgGear.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }
    }
}
