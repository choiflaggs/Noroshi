using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.UI;
using Noroshi.Game;
using Noroshi.Core.WebApi.Response.Character;

namespace Noroshi.CharacterList {
    public class CharacterPanel : MonoBehaviour {
        [SerializeField] Image imgCharacterIcon;
        [SerializeField] Image imgCharacterFrame;
        [SerializeField] Sprite[] imgFrameList;
        [SerializeField] Text txtLevel;
        [SerializeField] Text txtHaveStone;
        [SerializeField] Text txtNeedStone;
        [SerializeField] GameObject haveSoulWrapper;
        [SerializeField] GameObject levelTxtWrapper;
        [SerializeField] GameObject fullEvolutionTxt;
        [SerializeField] GameObject unlockTxt;
        [SerializeField] GameObject soulBar;
        [SerializeField] GameObject[] evolutionStar;
        [SerializeField] Image[] gearList;
        [SerializeField] Sprite[] gearStateList;
        [SerializeField] GameObject equipContainer;
        [SerializeField] GameObject evolutionContainer;
        [SerializeField] GameObject evolutionTxtContainer;
        [SerializeField] GameObject iconAttention;
        [SerializeField] Material grayScale;

        public class CharaData {
            public CharacterStatus status;
            public string name;
            public uint characterID;
            public uint playerCharacterID;
            public int lv;
            public int hp;
            public int power;
            public int exp;
            public int position;
            public ushort evolutionType;
            public int evolutionLv;
            public int promotionLv;
            public byte skinLv;
            public uint soulID;
            public int haveSoul;
            public int needSoul;
            public bool isDeca;
            public List<ushort> skillLvList = new List<ushort>();
            public List<uint> gearIDList = new List<uint>();
        }

        public Subject<CharacterStatus> OnChangedCharacterModel = new Subject<CharacterStatus>();
        public Subject<CharaData> OnOpenDetail = new Subject<CharaData>();
        public Subject<CharaData> OnUnlock = new Subject<CharaData>();
        public Subject<CharaData> OnOpenGetSoul = new Subject<CharaData>();

        public CharaData charaData = new CharaData();

        private bool canEquip;
        private bool canUnlock = false;
        private float soulBarWidth;

        private void DrawCharacterPanel() {
            imgCharacterIcon.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_{1}", charaData.characterID, charaData.skinLv)
            );
            if(charaData.promotionLv > 0) {
                imgCharacterFrame.sprite = imgFrameList[charaData.promotionLv - 1];
            }
            if(charaData.lv > 0) {
                txtLevel.text = charaData.lv.ToString();
                SetCharacterEvolutionLv();
                UPdateGear();
            } else {
                levelTxtWrapper.SetActive(false);
                SetUnAcquiredEvolutionLv();
            }
        }

        private void SetCharacterGear(uint id, int index) {
            charaData.gearIDList[index] = id;
            if(id > 0) {
                gearList[index].sprite = Resources.Load<Sprite>(
                    "Item/" + id
                );
            } else {
                GlobalContainer.RepositoryManager.CharacterRepository.Get(charaData.characterID).Do(masterData => {
                    var gID = masterData.GearIDs[charaData.promotionLv - 1][index];
                    
                    if(ItemListManager.Instance.GetItemCount(gID) > 0) { // have
                        if(charaData.lv >= ItemListManager.Instance.GetGearInfo(gID).Level) { // lv tariteru
                            canEquip = true;
                            iconAttention.SetActive(true);
                            gearList[index].sprite = gearStateList[0];
                        } else {
                            gearList[index].sprite = gearStateList[2];
                        }
                    } else {
                        if(CheckCreateGear(gID)) { // can create
                            if(charaData.lv >= ItemListManager.Instance.GetGearInfo(gID).Level) {
                                canEquip = true;
                                gearList[index].sprite = gearStateList[1];
                                iconAttention.SetActive(true);
                            } else {
                                gearList[index].sprite = gearStateList[2];
                            }
                        } else {
                            gearList[index].sprite = gearStateList[2];
                        }
                    }
                }).Subscribe();
            }
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

        private void SetCharacterEvolutionLv() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.CharacterEvolutionTypeRepository.GetByTypeAndLevel(
                charaData.evolutionType, (ushort)(charaData.evolutionLv + 1)
            ).Do(data => {
                charaData.needSoul = data != null ? (int)data.Soul : 20;
                txtHaveStone.text = charaData.haveSoul.ToString();
                txtNeedStone.text = charaData.needSoul.ToString();
                if(charaData.needSoul == 0) {
                    soulBar.transform.localPosition = Vector3.zero;
                    haveSoulWrapper.SetActive(false);
                    fullEvolutionTxt.SetActive(true);
                } else if(charaData.needSoul != 0 && charaData.haveSoul >= charaData.needSoul) {
                    soulBar.transform.localPosition = Vector3.zero;
                } else {
                    var xx = soulBarWidth - soulBarWidth * (float)charaData.haveSoul / (float)charaData.needSoul;
                    soulBar.transform.localPosition = new Vector3(-xx, 0, 0);
                }
                for(int i = 0, l = evolutionStar.Length; i < l; i++) {
                    if(i < (int)charaData.evolutionLv) {
                        evolutionStar[i].SetActive(true);
                    } else {
                        evolutionStar[i].SetActive(false);
                    }
                }
            }).Subscribe();
        }

        private void SetUnAcquiredEvolutionLv() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.CharacterEvolutionTypeRepository.GetByTypeAndLevel(
                charaData.evolutionType, (ushort)charaData.evolutionLv
            ).Do(data => {
                charaData.needSoul = data != null ? (int)data.Soul : 10;
                txtHaveStone.text = charaData.haveSoul.ToString();
                txtNeedStone.text = charaData.needSoul.ToString();
                if(charaData.needSoul <= charaData.haveSoul) {
                    soulBar.transform.localPosition = Vector3.zero;
                    haveSoulWrapper.SetActive(false);
                    unlockTxt.SetActive(true);
                    canUnlock = true;
                    iconAttention.SetActive(true);
                } else {
                    var xx = soulBarWidth - soulBarWidth * (float)charaData.haveSoul / (float)charaData.needSoul;
                    soulBar.transform.localPosition = new Vector3(-xx, 0, 0);
                    canUnlock = false;
                }
            }).Subscribe();
        }

        private void CheckRaiseEvolutionLv() {
            if(charaData.needSoul != 0 && charaData.haveSoul >= charaData.needSoul) {
                iconAttention.SetActive(true);
            } else {
                iconAttention.SetActive(false);
            }
        }

        private void ReDrawDetail() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.LoadCharacterStatusByPlayerCharacterID(charaData.playerCharacterID).Do(data => {
                charaData.lv = data.Level;
                charaData.hp = (int)data.MaxHP;
                charaData.power = (int)(data.PhysicalAttack + data.MagicPower + data.PhysicalCrit + data.MagicCrit);
                charaData.evolutionLv = data.EvolutionLevel;
                charaData.promotionLv = data.PromotionLevel;
                OnOpenDetail.OnNext(charaData);
            }).Subscribe();
        }
        
        public void SetCharacterPanelData(CharacterStatus characterStatus, uint id, int exp) {
            var gcrm = GlobalContainer.RepositoryManager;

            soulBarWidth = soulBar.GetComponent<RectTransform>().sizeDelta.x;
            charaData.status = characterStatus;
            charaData.characterID = characterStatus.CharacterID;
            charaData.playerCharacterID = id;
            charaData.lv = characterStatus.Level;
            charaData.hp = (int)characterStatus.MaxHP;
            charaData.power = (int)(characterStatus.PhysicalAttack + characterStatus.MagicPower + characterStatus.PhysicalCrit + characterStatus.MagicCrit);
            charaData.exp = exp;
            charaData.position = (int)characterStatus.Position;
            charaData.evolutionLv = characterStatus.EvolutionLevel;
            charaData.promotionLv = characterStatus.PromotionLevel;
            charaData.skinLv = characterStatus.SkinLevel;
            charaData.isDeca = characterStatus.TagSet.IsDeca;
            charaData.gearIDList = characterStatus.GearIDs.ToList();
            charaData.skillLvList = characterStatus.ActionLevels.ToList();
            charaData.skillLvList.RemoveAt(0);

            gcrm.CharacterRepository.Get(characterStatus.CharacterID).Do(masterData => {
                charaData.name = GlobalContainer.LocalizationManager.GetText(masterData.TextKey + ".Name");
                charaData.evolutionType = (ushort)masterData.EvolutionType;
                gcrm.SoulRepository.GetByCharacterId(characterStatus.CharacterID).Do(soul => {
                    charaData.soulID = soul.ID;
                    charaData.haveSoul = (int)ItemListManager.Instance.GetItemCount(soul.ID);
                    DrawCharacterPanel();
                }).Subscribe();
            }).Subscribe();
        }

        public void SetUnAcquiredCharacter(Character character) {
            var gcrm = GlobalContainer.RepositoryManager;

            soulBarWidth = soulBar.GetComponent<RectTransform>().sizeDelta.x;
//            imgCharacterIcon.material = grayScale;
            charaData.name = GlobalContainer.LocalizationManager.GetText(character.TextKey + ".Name");
            charaData.characterID = character.ID;
            charaData.evolutionType = character.EvolutionType;
            charaData.evolutionLv = character.InitialEvolutionLevel;
            charaData.skinLv = 1;
            charaData.position = (int)character.Position;
            gcrm.SoulRepository.GetByCharacterId(character.ID).Do(soul => {
                if(soul != null) {
                    charaData.soulID = soul.ID;
                    charaData.haveSoul = (int)ItemListManager.Instance.GetItemCount(soul.ID);
                }
                ChangeSort(1);
                DrawCharacterPanel();
            }).Subscribe();
        }

        public void OnClickedBtn() {
            if(canUnlock) {
                OnUnlock.OnNext(charaData);
            } else {
                OnOpenGetSoul.OnNext(charaData);
            }
            SelectCharacter();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        public void SelectCharacter() {
            OnOpenDetail.OnNext(charaData);
        }

        public void ChangeSort(int index) {
            if(index == 0) {
                equipContainer.SetActive(true);
                evolutionContainer.SetActive(false);
                evolutionTxtContainer.SetActive(false);
            } else if(index == 1) {
                equipContainer.SetActive(false);
                evolutionContainer.SetActive(true);
                evolutionTxtContainer.SetActive(true);
            }
        }

        public void UPdateGear(bool isOpen = false) {
            canEquip = false;
            CheckRaiseEvolutionLv();
            for(var i = 0; i < charaData.gearIDList.Count; i++) {
                SetCharacterGear(charaData.gearIDList[i], i);
            }
            if(isOpen) {ReDrawDetail();}
        }

        public void UpdateLevel(int lvValue, int expValue) {
            txtLevel.text = charaData.lv.ToString();
            UPdateGear();
            ReDrawDetail();
        }

        public void RaisePromotionLv() {
            imgCharacterFrame.sprite = imgFrameList[charaData.promotionLv - 1];
            UPdateGear();
            ReDrawDetail();
        }

        public void RaiseEvolutionLv() {
            SetCharacterEvolutionLv();
            if(!canEquip) {CheckRaiseEvolutionLv();}
            ReDrawDetail();
        }
    }
}