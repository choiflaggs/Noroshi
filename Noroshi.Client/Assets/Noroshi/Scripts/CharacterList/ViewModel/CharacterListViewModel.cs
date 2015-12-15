using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Game;
using Noroshi.UI;
using Noroshi.Core.WebApi.Response.Character;

namespace Noroshi.CharacterList {
    public class CharacterListViewModel : MonoBehaviour {
        [SerializeField] GameObject acquiredCharacterListContainer;
        [SerializeField] GameObject unAcquiredCharacterListContainer;
        [SerializeField] CharacterPanel characterPanel;
        [SerializeField] CharacterDetailPanel characterDetailPanel;
        [SerializeField] BtnCommon[] tabBtnList;
        [SerializeField] GameObject[] tabContentList;
        [SerializeField] BtnCommon btnOpenFilter;
        [SerializeField] Text txtBtnOpenFilter;
        [SerializeField] GameObject filterContainer;
        [SerializeField] GameObject filterBtnWrapper;
        [SerializeField] BtnCommon btnFilterBackground;
        [SerializeField] BtnCommon[] btnFilterList;
        [SerializeField] BtnCommon btnSort;
        [SerializeField] Text txtBtnSort;
        [SerializeField] CharacterEquipTab equipTab;
        [SerializeField] CharacterSkillTab skillTab;
        [SerializeField] CharacterStatusTab statusTab;
        [SerializeField] GetSoulPanel getSoulPanel;
        [SerializeField] CharacterListConfirm characterUnlockConfirm;
        [SerializeField] GameObject processing;

//        public static bool isDefaultOpen;
//
//        private static int selectedSort = 1;

        private List<uint> acquiredCharacterIDList = new List<uint>();
        private List<CharacterPanel> originCharacterList = new List<CharacterPanel>();
        private List<CharacterPanel> characterList = new List<CharacterPanel>();
        private List<CharacterPanel> unAcquiredCharacterList = new List<CharacterPanel>();
        private int selectedIndex;
        private int selectedFilter;
        private float filterPositionY = 9999;
        private bool isCharaCreated;

        private void Start() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());

            foreach(var tab in tabBtnList) {
                tab.OnClickedBtn.Subscribe(index => {
                    SwitchTab(index);
                });
                tab.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnOpenFilter.OnClickedBtn.Subscribe(_ => {
                filterContainer.SetActive(true);
                if(filterPositionY == 9999) {
                    filterPositionY = filterBtnWrapper.transform.localPosition.y;
                }
                filterBtnWrapper.SetActive(true);
                TweenA.Add(btnFilterBackground.gameObject, 0.1f, 0.5f).From(0).EaseOutCubic();
                TweenA.Add(filterBtnWrapper, 0.2f, 1).From(0).EaseOutCubic();
                TweenY.Add(filterBtnWrapper, 0.2f, filterPositionY).From(filterPositionY + 40).EaseOutCubic();
            });
            btnOpenFilter.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnFilterBackground.OnClickedBtn.Subscribe(_ => {
                CloseFilter();
            });

            foreach (var btn in btnFilterList) {
                btn.OnClickedBtn.Subscribe(id => {
                    selectedFilter = id;
                    FilterCharacterList(id);
                });
                btn.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnSort.OnClickedBtn.Subscribe(id => {
                SortCharacterList(id);
            });
            btnSort.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            characterDetailPanel.OnCloseDetail.Subscribe(_ => {
                acquiredCharacterListContainer.SetActive(true);
            });

            characterDetailPanel.OnSwitchDetail.Subscribe(direction => {
                selectedIndex += direction;
                if(selectedIndex < 0) {selectedIndex = characterList.Count - 1;}
                if(selectedIndex > characterList.Count - 1) {selectedIndex = 0;}
                characterList[selectedIndex].SelectCharacter();
            });

            characterDetailPanel.OnChangeExp.Subscribe(data => {
                characterList[selectedIndex].UpdateLevel(data.lv, data.exp);
                characterList[selectedIndex].SelectCharacter();
            });

            equipTab.OnClickedGetSoul.Subscribe(charaData => {
                getSoulPanel.OpenSoulPanel();
            });

            equipTab.OnEquip.Subscribe(id => {
                foreach (var panel in originCharacterList) {
                    var isOpen = panel.charaData.characterID == id;
                    panel.UPdateGear(isOpen);
                }
                characterDetailPanel.PlayCharacterAnimation(3);
            });

            equipTab.OnRaisePromotionLv.Subscribe(id => {
                characterList[selectedIndex].RaisePromotionLv();
                characterDetailPanel.PlayCharacterAnimation(3);
            });

            equipTab.OnRaiseEvolutionLv.Subscribe(evolutionLv => {
                characterList[selectedIndex].RaiseEvolutionLv();
                characterDetailPanel.SetCharacterSkin(evolutionLv);
                characterDetailPanel.PlayCharacterAnimation(3);
            });

            equipTab.OnCreate.Subscribe(_ => {
                foreach (var panel in originCharacterList) {
                    panel.UPdateGear();
                }
            });

            skillTab.OnSelectSkill.Subscribe(index => {
                int n;
                if(index == 0) {
                    n = 4;
                } else if(index == 1) {
                    n = 5;
                } else if(index == 2) {
                    n = 6;
                } else {
                    n = UnityEngine.Random.Range(4, 6);
                }
                characterDetailPanel.PlayCharacterAnimation(n);
            });

            characterUnlockConfirm.OnDecide.Subscribe(charaData => {
                var gcrm = GlobalContainer.RepositoryManager;
                processing.SetActive(true);
                gcrm.PlayerSoulRepository.UseSoulWithCreateCharacter(charaData.soulID).Do(_ => {
                    var getModal = Instantiate(Resources.Load<GetCharacterModal>("UI/GetCharacterModal"));
                    getModal.OpenModal(charaData.characterID);
                    ItemListManager.Instance.ChangeItemCount(charaData.soulID, -charaData.needSoul);
                    foreach(var charaPanel in unAcquiredCharacterList.Where(chara => chara.charaData.characterID == charaData.characterID)) {
                        charaPanel.gameObject.SetActive(false);
                    }
                    gcrm.PlayerCharacterRepository.GetAll().Do(charaList => {
                        foreach(var chara in charaList.Where(chara => chara.CharacterID == charaData.characterID)) {
                            gcrm.LoadCharacterStatusByPlayerCharacterID(chara.ID).Do(c => {
                                CreateCharacterList(c, chara.ID, 0);
                                FilterCharacterList(selectedFilter);
                                characterDetailPanel.CloseCharacterDetail();
                                processing.SetActive(false);
                            }).Subscribe();
                        }
                    }).Subscribe();
                }).Subscribe();
            });

            SwitchTab(0);
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while (!ItemListManager.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            LoadCharacterList();
            while (!isCharaCreated) {
                yield return new WaitForEndOfFrame();
            }
            characterDetailPanel.CloseCharacterDetail();
            characterDetailPanel.Init();
            if(UILoading.Instance.GetQuery(QueryKeys.SelectedCharacter) > -1) {
                SetDefaultAcquiredCharacterOpen();
            } else if(UILoading.Instance.GetQuery(QueryKeys.SelectedUnAcquiredCharacter) > -1) {
                SetDefaultUnAcquiredCharacterOpen();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
            if (SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.CHARACTER_LIST, true);
            }
        }

        private void SetDefaultAcquiredCharacterOpen() {
            var charaID = UILoading.Instance.GetQuery(QueryKeys.SelectedCharacter);
            var equipIndex = UILoading.Instance.GetQuery(QueryKeys.SelectedEquipIndex);
            foreach(var chara in characterList.Where(chara => chara.charaData.characterID == charaID)) {
                chara.SelectCharacter();
            }
            if(equipIndex > -1) {
                equipTab.OpenDefaultGearPanel(equipIndex);
            }
            if(UILoading.Instance.GetQuery(QueryKeys.IsOpenGetSoulPanel) > -1) {
                equipTab.OpenDefaultGetSoulPanel();
            }
        }

        private void SetDefaultUnAcquiredCharacterOpen() {
            var charaID = UILoading.Instance.GetQuery(QueryKeys.SelectedUnAcquiredCharacter);
            SwitchTab(1);
            foreach(var chara in unAcquiredCharacterList.Where(chara => chara.charaData.characterID == charaID)) {
                chara.OnOpenGetSoul.OnNext(chara.charaData);
            }
        }

        private void LoadCharacterList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(list => {
                foreach(var chara in list) {
                    var c = chara;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(chara.ID).Do(character => {
                        gcrm.LevelMasterRepository.GetCharacterLevel((ushort)character.Level).Do(lv => {
                            acquiredCharacterIDList.Add(c.CharacterID);
                            CreateCharacterList(character, c.ID, (int)(lv.Exp - c.ExpInLevel));
                            if(originCharacterList.Count == list.Length) {
                                characterList = originCharacterList;
                                LoadUnAcquiredCharacterList();
                                FilterCharacterList(0);
                                SortCharacterList(1);
                            }
                        }).Subscribe();
                    }).Subscribe();
                }
            }).Subscribe();
        }

        private void LoadUnAcquiredCharacterList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAllCharacters().Do(allList => {
                for(int i = 0, l = allList.Length; i < l; i++) {
                    gcrm.SoulRepository.GetByCharacterId(allList[i].CharacterID).Do(soul => {
                        if(!acquiredCharacterIDList.Contains(allList[i].CharacterID)
                           && soul != null
                           ) {
                            gcrm.CharacterRepository.Get(allList[i].CharacterID).Do(character => {
                                CreateUnAcquiredCharacterList(character);
                            }).Subscribe();
                        }
                        if(i == l - 1) {
                            unAcquiredCharacterList = unAcquiredCharacterList
                                .OrderByDescending(c => ((float)c.charaData.haveSoul / (float)c.charaData.needSoul))
                                    .ThenBy(c => c.charaData.characterID).ToList();
                            foreach(var chara in unAcquiredCharacterList) {
                                chara.transform.SetAsLastSibling();
                            }
                        }
                    }).Subscribe();
                }
                isCharaCreated = true;
            }).Subscribe();
        }

        private void CreateCharacterList(CharacterStatus character, uint id, int exp) {
            var charaPanel = Instantiate(characterPanel);
            charaPanel.transform.SetParent(acquiredCharacterListContainer.transform);
            charaPanel.transform.localScale = Vector3.one;
            charaPanel.SetCharacterPanelData(character, id, exp);
            charaPanel.OnOpenDetail.Subscribe(data => {
                UILoading.Instance.SetQuery(QueryKeys.SelectedCharacter, (int)data.characterID);
                characterDetailPanel.OpenCharacterDetail(data);
                equipTab.SetData(data);
                skillTab.SetData(data);
                statusTab.SetData(data);
                FindIndex(data.characterID);
                TweenNull.Add(gameObject, 0.15f).Then(() => {
                    acquiredCharacterListContainer.SetActive(false);
                });
            });

            originCharacterList.Add(charaPanel);
            characterDetailPanel.CreateCharaImage(character);
        }

        private void CreateUnAcquiredCharacterList(Character character) {
            var gcrm = GlobalContainer.RepositoryManager;
            var charaPanel = Instantiate(characterPanel);
            charaPanel.transform.SetParent(unAcquiredCharacterListContainer.transform);
            charaPanel.transform.localScale = Vector3.one;
            charaPanel.SetUnAcquiredCharacter(character);
            charaPanel.OnUnlock.Subscribe(charaData => {
                gcrm.CharacterEvolutionTypeRepository.GetByTypeAndLevel(
                    charaData.evolutionType, (ushort)charaData.evolutionLv
                ).Do(data => {
                    characterUnlockConfirm.OpenConfirm(charaData, data.NecessaryGold);
                }).Subscribe();
            });
            charaPanel.OnOpenGetSoul.Subscribe(charaData => {
                UILoading.Instance.SetQuery(QueryKeys.SelectedUnAcquiredCharacter, (int)charaData.characterID);
                getSoulPanel.OpenSoulPanel();
            });

            unAcquiredCharacterList.Add(charaPanel);
        }

        private void FindIndex(uint charaID) {
            for(int i = 0, l = characterList.Count; i < l; i++) {
                if(characterList[i].charaData.characterID == charaID) {
                    selectedIndex = i;
                    return;
                }
            }
        }

        private void SwitchTab(int index) {
            for(int i = 0, l = tabContentList.Length; i < l; i++) {
                if(i == index) {
                    tabBtnList[i].transform.localScale = Vector3.one;
                    tabBtnList[i].SetSelect(true);
                    TweenA.Add(tabBtnList[i].gameObject, 0.01f, 1);
                    tabContentList[i].SetActive(true);
                } else {
                    tabBtnList[i].transform.localScale = Vector3.one * 0.9f;
                    tabBtnList[i].SetSelect(false);
                    TweenA.Add(tabBtnList[i].gameObject, 0.01f, 0.7f);
                    tabContentList[i].SetActive(false);
                }
            }
        }

        private void FilterCharacterList(int n) {
            var gclm = GlobalContainer.LocalizationManager;
            var tempList = new List<CharacterPanel>();

            foreach(var chara in originCharacterList) {
                if(n == 0 || (n < 4 && n == chara.charaData.position) || (n == 4 && chara.charaData.isDeca)) {
                    tempList.Add(chara);
                    chara.gameObject.SetActive(true);
                } else {
                    chara.gameObject.SetActive(false);
                }
            }
            characterList = tempList;
            for(int i = 0, l = btnFilterList.Length; i < l; i++) {
                if (i == n) {
                    btnFilterList[i].SetSelect(true);
                } else {
                    btnFilterList[i].SetSelect(false);
                }
            }
            switch (n) {
                case 0: txtBtnOpenFilter.text = gclm.GetText("UI.Button.All"); break;
                case 1: txtBtnOpenFilter.text = gclm.GetText("UI.Button.Front"); break;
                case 2: txtBtnOpenFilter.text = gclm.GetText("UI.Button.Central"); break;
                case 3: txtBtnOpenFilter.text = gclm.GetText("UI.Button.Back"); break;
                case 4: txtBtnOpenFilter.text = gclm.GetText("UI.Button.Deca"); break;
                default: break;
            }
            CloseFilter();
        }

        private void CloseFilter() {
            TweenA.Add(btnFilterBackground.gameObject, 0.25f, 0);
            TweenY.Add(filterBtnWrapper, 0.25f, filterPositionY + 40).EaseOutCubic();
            TweenA.Add(filterBtnWrapper, 0.25f, 0).EaseOutCubic().Then(() => {
                filterBtnWrapper.SetActive(false);
                filterContainer.gameObject.SetActive(false);
            });
        }

        private void SortCharacterList(int type) {
            type++;
            if(type > 1) {type = 0;}
            btnSort.id = type;

            switch (type) {
                case 0:
                    txtBtnSort.text = GlobalContainer.LocalizationManager.GetText("UI.Heading.Level");
                    characterList = characterList
                        .OrderByDescending(c => c.charaData.lv).ThenBy(c => c.charaData.characterID).ToList();
                    originCharacterList = originCharacterList
                        .OrderByDescending(c => c.charaData.lv).ThenBy(c => c.charaData.characterID).ToList();
                    break;
                case 1:
                    txtBtnSort.text = GlobalContainer.LocalizationManager.GetText("UI.Button.Evolution");
                    characterList = characterList
                        .OrderByDescending(c => c.charaData.evolutionLv).ThenBy(c => c.charaData.characterID).ToList();
                    originCharacterList = originCharacterList
                        .OrderByDescending(c => c.charaData.evolutionLv).ThenBy(c => c.charaData.characterID).ToList();
                    break;
                default:
                    break;
            }
            acquiredCharacterListContainer.SetActive(false);
            foreach(var chara in originCharacterList) {
                chara.ChangeSort(type);
                chara.transform.SetAsLastSibling();
            }
            acquiredCharacterListContainer.SetActive(true);
        }

        public bool CheckCharacterCreated() {
            return isCharaCreated;
        }
    }
}