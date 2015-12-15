using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Game;
using Noroshi.UI;
using Noroshi.Core.Game.Player;

namespace Noroshi.CharacterList {
    public class CharacterEquipTab : MonoBehaviour {
        [SerializeField] Text txtCareerName;
        [SerializeField] GameObject[] starList;
        [SerializeField] Text txtHaveStone;
        [SerializeField] Text txtNeedStone;
        [SerializeField] GameObject stoneTxtSeparator;
        [SerializeField] GameObject fullEvolutionTxt;
        [SerializeField] GameObject soulBar;
        [SerializeField] BtnCommon btnGetSoul;
        [SerializeField] BtnEquip[] btnEquipList;
        [SerializeField] BtnCommon btnRaisePromotionLv;
        [SerializeField] GearDetail gearDetail;
        [SerializeField] BtnCommon btnRaiseEvolutionLv;
        [SerializeField] CharacterListConfirm evolutionConfirm;
        [SerializeField] CharacterListConfirm promotionConfirm;

        public Subject<CharacterPanel.CharaData> OnClickedGetSoul = new Subject<CharacterPanel.CharaData>();
        public Subject<uint> OnEquip = new Subject<uint>();
        public Subject<uint> OnRaisePromotionLv = new Subject<uint>();
        public Subject<int> OnRaiseEvolutionLv = new Subject<int>();
        public Subject<bool> OnCreate = new Subject<bool>();

        private CharacterPanel.CharaData charaData;
        private Dictionary<string, Core.WebApi.Response.Gear> detailGearList;
        private int selectedIndex;
        private float soulBarWidth;

        private void Start() {
            soulBarWidth = soulBar.GetComponent<RectTransform>().sizeDelta.x;

            foreach (var btn in btnEquipList) {
                btn.OnClickedBtn.Subscribe(index => {
                    var data = detailGearList.ContainsKey("gear" + index) ?
                        detailGearList["gear" + index] : null;
                    selectedIndex = index;
                    UILoading.Instance.SetQuery(QueryKeys.SelectedEquipIndex, index);
                    gearDetail.OpenGearDetail(data, btnEquipList[index].equipState, charaData.lv);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnGetSoul.OnClickedBtn.Subscribe(_ => {
                UILoading.Instance.SetQuery(QueryKeys.IsOpenGetSoulPanel, 1);
                OnClickedGetSoul.OnNext(charaData);
            });

            gearDetail.OnEquip.Subscribe(gearID => {
                var gcrm = GlobalContainer.RepositoryManager;
                gcrm.PlayerCharacterRepository.EquipGear(
                    charaData.playerCharacterID, gearID, (byte)(selectedIndex + 1)
                ).Do(data => {
                    charaData.gearIDList[selectedIndex] = gearID;
                    btnEquipList[selectedIndex].equipState = (int)gearID;
                    OnEquip.OnNext(charaData.characterID);
                }).Subscribe();
            });

            gearDetail.OnCreate.Subscribe(id => {
                var gcrm = GlobalContainer.RepositoryManager;
                gcrm.PlayerGearRepository.GearCraft(id).Do(_ => {
                    gearDetail.ReDraw(id);
                    SetEquipList();
                    OnCreate.OnNext(true);
                }).Subscribe();
            });

            btnRaisePromotionLv.OnClickedBtn.Subscribe(_ => {
                promotionConfirm.OpenConfirm(charaData, 0);
            });

            promotionConfirm.OnDecide.Subscribe(data => {
                var gcrm = GlobalContainer.RepositoryManager;
                gcrm.PlayerCharacterRepository.UpPromotionLevel(charaData.playerCharacterID).Do(_ => {
                    charaData.promotionLv++;
                    charaData.gearIDList = new List<uint> {0, 0, 0, 0, 0, 0};
                    btnRaisePromotionLv.gameObject.SetActive(false);
                    OnRaisePromotionLv.OnNext(charaData.characterID);
                }).Subscribe();
            });
            promotionConfirm.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.UPGRADE);
            });

            btnRaiseEvolutionLv.OnClickedBtn.Subscribe(_ => {
                var gcrm = GlobalContainer.RepositoryManager;
                gcrm.CharacterEvolutionTypeRepository.GetByTypeAndLevel(
                    charaData.evolutionType, (ushort)(charaData.evolutionLv + 1)
                ).Do(data => {
                    evolutionConfirm.OpenConfirm(charaData, data.NecessaryGold);
                }).Subscribe();
            });

            evolutionConfirm.OnDecide.Subscribe(data => {
                var gcrm = GlobalContainer.RepositoryManager;
                gcrm.PlayerSoulRepository.UseSoulWithEvolutionLevel(charaData.soulID).Do(_ => {
                    charaData.evolutionLv++;
                    charaData.haveSoul -= charaData.needSoul;
                    gcrm.CharacterEvolutionTypeRepository.GetByTypeAndLevel(
                        charaData.evolutionType, (ushort)charaData.evolutionLv
                    ).Do(d => {
                        charaData.needSoul = d != null ? (int)d.Soul : 50;
                        OnRaiseEvolutionLv.OnNext(charaData.evolutionLv);
                    }).Subscribe();
                }).Subscribe();
            });
            evolutionConfirm.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.EVOLUTION);
            });
        }

        private bool CheckCreateGear(uint id) {
            var recipes = ItemListManager.Instance.GetGearRecipe(id);
            var isCreate = true;
            if(recipes.Count < 1) {
                isCreate = false;
            } else {
                foreach(var recipe in recipes) {
                    if(recipe["num"] > ItemListManager.Instance.GetItemCount(recipe["id"])) {
                        if(ItemListManager.Instance.GetGearRecipe(recipe["id"]) != null) {
                            isCreate = CheckCreateGear(recipe["id"]);
                            if(!isCreate) {break;}
                        } else {
                            isCreate = false;
                            break;
                        }
                    }
                }
            }
            return isCreate;
        }

        private void SetEquipList() {
            var gcrm = GlobalContainer.RepositoryManager;
            var isPromotion = true;

            detailGearList = new Dictionary<string, Core.WebApi.Response.Gear>();
            if(charaData.promotionLv > 12 - 1) {isPromotion = false;}

            for(int i = 0, l = btnEquipList.Length; i < l; i++) {
                int index = i;
                if(charaData.gearIDList[index] > 0) {
                    btnEquipList[index].SetGearImage(charaData.gearIDList[index], 1);
                    detailGearList["gear" + index] = ItemListManager.Instance.GetGearInfo(charaData.gearIDList[index]);
                } else {
                    isPromotion = false;
                    gcrm.CharacterRepository.Get(charaData.characterID).Do(masterData => {
                        var gID = masterData.GearIDs[charaData.promotionLv - 1][index];

                        detailGearList["gear" + index] = ItemListManager.Instance.GetGearInfo(gID);
                        if(ItemListManager.Instance.GetItemCount(gID) > 0) { // have
                            if(charaData.lv >= ItemListManager.Instance.GetGearInfo(gID).Level) { // lv tariteru
                                btnEquipList[index].SetGearImage(gID, -2);
                            } else {
                                btnEquipList[index].SetGearImage(gID, -1);
                            }
                        } else {
                            if(CheckCreateGear(gID)) { // can create
                                if(charaData.lv >= ItemListManager.Instance.GetGearInfo(gID).Level) { 
                                    btnEquipList[index].SetGearImage(gID, -3);
                                } else {
                                    btnEquipList[index].SetGearImage(gID, -4);
                                }
                            } else {
                                btnEquipList[index].SetGearImage(gID, 0);
                            }
                        }
                    }).Subscribe();
                }
            }
            if(isPromotion && PlayerInfo.Instance.GetTutorialStep() >= TutorialStep.ClearStoryStage4) {
                btnRaisePromotionLv.gameObject.SetActive(true);
            } else {
                btnRaisePromotionLv.gameObject.SetActive(false);
            }
        }

        private void SetEvolution() {
            txtHaveStone.text = charaData.haveSoul.ToString();
            txtNeedStone.text = charaData.needSoul.ToString();
            if(charaData.needSoul == 0) {
                soulBar.transform.localPosition = Vector3.zero;
                txtHaveStone.gameObject.SetActive(false);
                txtNeedStone.gameObject.SetActive(false);
                stoneTxtSeparator.SetActive(false);
                fullEvolutionTxt.SetActive(true);
            } else {
                txtHaveStone.gameObject.SetActive(true);
                txtNeedStone.gameObject.SetActive(true);
                stoneTxtSeparator.SetActive(true);
                fullEvolutionTxt.SetActive(false);
                if(charaData.haveSoul < charaData.needSoul) {
                    var xx = soulBarWidth - soulBarWidth * (float)charaData.haveSoul / (float)charaData.needSoul;
                    soulBar.transform.localPosition = new Vector3(-xx, 0, 0);
                } else {
                    soulBar.transform.localPosition = Vector3.zero;
                }
            }
            if(charaData.haveSoul < charaData.needSoul || charaData.needSoul == 0 ||
                PlayerInfo.Instance.GetTutorialStep() < TutorialStep.ClearStoryStage10) {
                btnRaiseEvolutionLv.gameObject.SetActive(false);
            } else {
                btnRaiseEvolutionLv.gameObject.SetActive(true);
            }
            for(int i = 0, l = starList.Length; i < l; i++) {
                if(i < charaData.evolutionLv) {
                    starList[i].SetActive(true);
                } else {
                    starList[i].SetActive(false);
                }
            }
        }

        public void SetData(CharacterPanel.CharaData data) {
            charaData = data;
            SetEquipList();
            SetEvolution();
        }

        public void OpenDefaultGearPanel(int index) {
            var ids = UILoading.Instance.GetMultiQuery(QueryKeys.SelectedEquipGearList);
            btnEquipList[index].OnClickedBtn.OnNext(index);
            if(ids.Count > 0) {
                gearDetail.SetDefault(ids);
            }
        }

        public void OpenDefaultGetSoulPanel() {
            btnGetSoul.OnClickedBtn.OnNext(1);
        }
    }
}
