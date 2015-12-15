using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using Noroshi.Game;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class GearDetail : MonoBehaviour {
        [SerializeField] BtnCommon btnBackground;
        [SerializeField] GearStatusPanel gearStatusPanel;
        [SerializeField] GearRecipePanel gearRecipePanel;
        [SerializeField] GearGetPanel gearGetPanel;

        public Subject<uint> OnEquip = new Subject<uint>();
        public Subject<uint> OnCreate = new Subject<uint>();

        private List<GameObject> childPanelList = new List<GameObject>();
        private List<int> selectedGearIDList = new List<int>();
        private Core.WebApi.Response.Gear gearData;
        private int depth = 0;
        private int characterLv;

        private void Start() {
            btnBackground.OnClickedBtn.Subscribe(_ => {
                CloseGearSet();
            });
            btnBackground.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            gearStatusPanel.OnOpenRecipe.Subscribe(_ => {
                if(childPanelList.Count > 0) {return;}
                OpenRecipe(gearData.ID);
            });

            gearStatusPanel.OnEquip.Subscribe(gearID => {
                OnEquip.OnNext(gearID);
                CloseGearSet();
            });
        }

        private void OpenRecipe(uint id) {
            var recipes = ItemListManager.Instance.GetGearRecipe(id);

            depth++;
            if(recipes != null && recipes.Count > 0) {
                CreateRecipePanel(id);
            } else {
                CreateGetPanel(id);
            }
            selectedGearIDList.Add((int)id);
            UILoading.Instance.SetMultiQuery(QueryKeys.SelectedEquipGearList, selectedGearIDList);
            MoveStatusPanel();
            MoveChildPanelList();
        }

        private void CreateRecipePanel(uint id) {
            var panel = Instantiate(gearRecipePanel);
            panel.transform.SetParent(transform);
            panel.transform.SetAsFirstSibling();
            btnBackground.transform.SetAsFirstSibling();
            panel.transform.localScale = Vector3.one;
            panel.transform.localPosition = new Vector3(0, -30, 0);
            childPanelList.Add(panel.gameObject);
            panel.OnMaterialSelect.Subscribe(materialID => {
                OpenRecipe(materialID);
            }).AddTo(panel);
            panel.OnBack.Subscribe(_ => {
                IncreaseDepth();
            }).AddTo(panel);
            panel.OnCreate.Subscribe(craftID => {
                IncreaseDepth();
                OnCreate.OnNext(craftID);
            }).AddTo(panel);
            panel.SetCreateGearInfo(id);
        }

        private void CreateGetPanel(uint id) {
            var panel = Instantiate(gearGetPanel);
            panel.transform.SetParent(transform);
            panel.transform.SetAsFirstSibling();
            btnBackground.transform.SetAsFirstSibling();
            panel.transform.localScale = Vector3.one;
            panel.transform.localPosition = new Vector3(0, -30, 0);
            childPanelList.Add(panel.gameObject);
            panel.OnBack.Subscribe(_ => {
                IncreaseDepth();
            }).AddTo(panel);
            panel.SetInfo(id);
        }

        private void IncreaseDepth() {
            depth--;
            Destroy(childPanelList[childPanelList.Count - 1]);
            childPanelList.RemoveAt(childPanelList.Count - 1);
            MoveStatusPanel();
            MoveChildPanelList();
        }

        private void MoveStatusPanel() {
            if(depth == 0) {
                TweenX.Add(gearStatusPanel.gameObject, 0.2f, 0).EaseOutCubic();
            } else {
                TweenX.Add(gearStatusPanel.gameObject, 0.2f, -175 - depth * 25).EaseOutCubic();
            }
        }

        private void MoveChildPanelList() {
            for(int i = 0, l = childPanelList.Count; i < l; i++) {
                if(i < l - 1) {
                    TweenX.Add(childPanelList[i], 0.2f, -depth * 25 - 155 + i * 50).EaseOutCubic();
                } else {
                    TweenX.Add(childPanelList[i], 0.2f, 180 + depth * 25).EaseOutCubic();
                }
            }
        }
        
        private void CloseGearSet() {
            depth = 0;
            selectedGearIDList = new List<int>();
            UILoading.Instance.RemoveQuery(QueryKeys.SelectedEquipGearList);
            foreach(var panel in childPanelList) {
                Destroy(panel);
            }
            childPanelList = new List<GameObject>();
            gearStatusPanel.CloseGearStatus();
            gameObject.SetActive(false);
        }

        public void OpenGearDetail(Core.WebApi.Response.Gear gData, int state, int lv) {
            gearData = gData;
            characterLv = lv;

            gameObject.SetActive(true);
            gearStatusPanel.SetGearStatusPanel(gearData, state, lv);
        }

        public void ReverseGearSet() {
            TweenX.Add(gearStatusPanel.gameObject, 0.2f, 0).EaseOutCubic();
        }

        public void ReDraw(uint createID) {
            if (createID == gearData.ID) {
                gearStatusPanel.SetGearStatusPanel(gearData, -1, characterLv);
            }
        }

        public void SetDefault(List<int> ids) {
            for(int i = 0, l = ids.Count; i < l; i++) {
                OpenRecipe((uint)ids[i]);
            }
        }
    }
}
