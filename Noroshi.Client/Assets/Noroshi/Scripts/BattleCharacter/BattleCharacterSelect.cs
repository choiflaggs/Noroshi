using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using LitJson;
using Noroshi.Game;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Core.WebApi.Response.Battle;

namespace Noroshi.UI {
    public class BattleCharacterSelect : MonoBehaviour {
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject modal;
        [SerializeField] RectTransform characterListContainer;
        [SerializeField] ScrollRect ownCharacterList;
        [SerializeField] ScrollRect mercenaryList;
        [SerializeField] GameObject characterIconListWrapper;
        [SerializeField] GameObject characterPanelListWrapper;
        [SerializeField] GameObject mercenaryIconListWrapper;
        [SerializeField] CharacterSelectPanel selectPanel;
        [SerializeField] CharacterSelectIcon selectIcon;
        [SerializeField] MercenarySelectIcon mercenarySelectIcon;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] BtnCommon btnBattle;
        [SerializeField] BtnCommon btnOpenFilter;
        [SerializeField] Text txtBtnOpenFilter;
        [SerializeField] GameObject filterContainer;
        [SerializeField] GameObject filterBtnWrapper;
        [SerializeField] BtnCommon btnFilterBackground;
        [SerializeField] BtnCommon[] btnFilterList;
        [SerializeField] BtnCommon[] tabBtnList;
        [SerializeField] GameObject[] tabContentList;
        [SerializeField] BtnCommon btnSort;
        [SerializeField] Text txtBtnSort;
        [SerializeField] Text txtSelectedPower;
        [SerializeField] Text txtSelectedHp;
        [SerializeField] AlertModal decaCharaAlert;
        [SerializeField] AlertModal requireCharaAlert;
        [SerializeField] AlertModal mercenaryAlert;
        [SerializeField] AlertModal sameCharacterAlert;
        [SerializeField] GameObject noticeCharacterSelect;

        public static BattleCharacterSelect Instance;

        public Subject<uint[]> OnStartBattle = new Subject<uint[]>();
        public Subject<uint[]> OnClosePanel = new Subject<uint[]>();

        public bool isLoad = false;

        private List<CharacterSelectIcon> characterIconList = new List<CharacterSelectIcon>();
        private List<CharacterSelectPanel> characterPanelList = new List<CharacterSelectPanel>();
        private List<CharacterSelectIcon> mercenaryIconList = new List<CharacterSelectIcon>();
        private List<CharacterSelectPanel> mercenaryPanelList = new List<CharacterSelectPanel>();
        private List<CharacterSelectIcon> cpuIconList = new List<CharacterSelectIcon>();
        private List<CharacterSelectPanel> cpuPanelList = new List<CharacterSelectPanel>();
        private List<uint> battleCharacterIdList = new List<uint>();
        private List<CharacterSelectPanel> selectedCharacterList = new List<CharacterSelectPanel>();
        private float filterPositionY = 9999;
        private int selectedPower = 0;
        private int selectedHp = 0;
        private int sortTypeLength = 3;
        private int maxBattleCharacterNum = 5;
        private int maxDecaCharacterNum = 2;
        private int maxMercenaryNum = 1;
        private int crntDecaCharacterNum = 0;
        private int crntMercenaryNum = 0;
        private bool isMercenaryOpen = false;

        private Vector2[] selectedPositionList = new Vector2[] {
            new Vector2(390, 170),
            new Vector2(187.5f, 170),
            new Vector2(-15, 170),
            new Vector2(-217.5f, 170),
            new Vector2(-420, 170)
        };

        private void Awake() {
            if (Instance == null) {Instance = this;}
            GameObject[] obj = GameObject.FindGameObjectsWithTag("BattleCharacterSelect");
            if(obj.Length > 1) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }
            canvas.worldCamera = Camera.main;
        }

        private void Start() {
            var ratio =  (float)Screen.height / (float)Screen.width;
            characterListContainer.sizeDelta = new Vector2(
                characterListContainer.sizeDelta.x,
                (Constant.SCREEN_BASE_WIDTH * ratio - Constant.SCREEN_BASE_HEIGHT) / 2 + characterListContainer.sizeDelta.y
            );

            btnClose.OnClickedBtn.Subscribe(_ => {
                ClosePanel(true);
            });
            btnClose.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnOK.OnClickedBtn.Subscribe(_ => {
                ClosePanel(false);
            });
            btnOK.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnBattle.OnClickedBtn.Subscribe(_ => {
                StartBattle();
            });
            btnBattle.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.StopBGM();
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });

            foreach(var tab in tabBtnList) {
                tab.OnClickedBtn.Subscribe(index => {
                    SwitchTab(index);
                });
                tab.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnOpenFilter.OnClickedBtn.Subscribe(_ => {
                if(isMercenaryOpen) {return;}
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

            foreach(var btn in btnFilterList) {
                btn.OnClickedBtn.Subscribe(id => {
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
        }

        private void OnLevelWasLoaded(int level) {
            ClosePanel();
            canvas.worldCamera = Camera.main;
        }

        private void CreateCharacterList(CharacterStatus characterStatus, uint pcID) {
            var icon = Instantiate(selectIcon);
            var panel = Instantiate(selectPanel);

            characterIconList.Add(icon);
            icon.SetInfo(characterStatus, pcID);
            icon.transform.SetParent(characterIconListWrapper.transform);
            icon.transform.localScale = Vector3.one;
            icon.OnShowRequire.Subscribe(_ => {
                requireCharaAlert.OnOpen();
            });
            SetIconEvent(icon);

            characterPanelList.Add(panel);
            panel.transform.SetParent(characterPanelListWrapper.transform);
            panel.Init(characterStatus, pcID);
            panel.InActiveCharacter();
            panel.OnShowRequire.Subscribe(_ => {
                requireCharaAlert.OnOpen();
            });
            SetPanelEvent(panel, icon);
        }

        private void CreateMercenaryList(CharacterStatus characterStatus, uint pcID) {
            var icon = Instantiate(mercenarySelectIcon);
            var panel = Instantiate(selectPanel);
            
            mercenaryIconList.Add(icon);
            icon.SetInfo(characterStatus, pcID);
            icon.SetMercenaryInfo("Player" + pcID, 10000);
            icon.transform.SetParent(mercenaryIconListWrapper.transform);
            icon.transform.localScale = Vector3.one;
            icon.OnShowRequire.Subscribe(_ => {
                requireCharaAlert.OnOpen();
            });
            SetIconEvent(icon, true);
            
            mercenaryPanelList.Add(panel);
            panel.transform.SetParent(characterPanelListWrapper.transform);
            panel.Init(characterStatus, pcID);
            panel.InActiveCharacter();
            SetPanelEvent(panel, icon, true);
        }

        private void CreateCPUCharacter(uint characterID) {
            var icon = Instantiate(selectIcon);
            var panel = Instantiate(selectPanel);

            cpuIconList.Add(icon);
            icon.SetCPU(characterID);
            icon.transform.SetParent(characterIconListWrapper.transform);
            icon.transform.localScale = Vector3.one;

            cpuPanelList.Add(panel);
            panel.transform.SetParent(characterPanelListWrapper.transform);
            var position = selectedPositionList[maxBattleCharacterNum - cpuPanelList.Count];
            panel.SetCPU(characterID, position);
        }

        private void SetIconEvent(CharacterSelectIcon icon, bool isMercenary = false) {
            icon.OnClickedIcon.Subscribe(playerCharacterID => {
                if(battleCharacterIdList.Contains(playerCharacterID)) {
                    icon.OnSelect(false);
                    if(icon.isDeca) {crntDecaCharacterNum--;}
                    if(isMercenary) {crntMercenaryNum--;}
                    SetSelectedStatus(-icon.power, -icon.hp);
                    RemoveSelectedCharacter(playerCharacterID);
                    MoveSelectedCharacter();
                    SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
                } else {
                    if(isMercenary) {
                        if(crntMercenaryNum > maxMercenaryNum - 1) {
                            mercenaryAlert.OnOpen();
                            return;
                        } else {
                            crntMercenaryNum++;
                        }
                    }
                    if(icon.isDeca) {
                        if(crntDecaCharacterNum > maxDecaCharacterNum - 1) {
                            decaCharaAlert.OnOpen();
                            return;
                        } else {
                            crntDecaCharacterNum++;
                        }
                    }
                    for(int i = 0, l = selectedCharacterList.Count; i < l; i++) {
                        if(selectedCharacterList[i].id == icon.id) {
                            sameCharacterAlert.OnOpen();
                            return;
                        }
                    }
                    AddSelectedCharacter(icon);
                    MoveSelectedCharacter();
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                }
            });
        }

        private void SetPanelEvent(CharacterSelectPanel panel, CharacterSelectIcon icon, bool isMercenary = false) {
            panel.OnClickedPanel.Subscribe(playerCharacterID => {
                if(battleCharacterIdList.Contains(playerCharacterID)) {
                    icon.OnSelect(false);
                    if(icon.isDeca) {crntDecaCharacterNum--;}
                    if(isMercenary) {crntMercenaryNum--;}
                    SetSelectedStatus(-icon.power, -icon.hp);
                    RemoveSelectedCharacter(playerCharacterID);
                    MoveSelectedCharacter();
                    SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
                }
            });
        }
        
        private void FilterCharacterList(int n) {
            var gclm = GlobalContainer.LocalizationManager;
            foreach(var chara in characterIconList) {
                if(n == 0 || (n < 4 && n == chara.position) || (n == 4 && chara.isDeca)) {
                    chara.gameObject.SetActive(true);
                } else {
                    chara.gameObject.SetActive(false);
                }
            }
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
            var gclm = GlobalContainer.LocalizationManager;
            type++;
            if (type > sortTypeLength - 1) {type = 0;}
            btnSort.id = type;

            ownCharacterList.gameObject.SetActive(false);
            mercenaryList.gameObject.SetActive(false);
            switch (type) {
                case 0:
                    txtBtnSort.text = gclm.GetText("UI.Heading.Level");
                    characterIconList = characterIconList
                        .OrderByDescending(c => c.lv).ThenBy(c => c.id).ToList();
                    mercenaryIconList = mercenaryIconList
                        .OrderByDescending(c => c.lv).ThenBy(c => c.id).ToList();
                    break;
                case 1:
                    txtBtnSort.text = gclm.GetText("UI.Heading.HP");
                    characterIconList = characterIconList
                        .OrderByDescending(c => c.hp).ThenBy(c => c.id).ToList();
                    mercenaryIconList = mercenaryIconList
                        .OrderByDescending(c => c.hp).ThenBy(c => c.id).ToList();
                    break;
                case 2:
                    txtBtnSort.text = gclm.GetText("UI.Heading.Power");
                    characterIconList = characterIconList
                        .OrderByDescending(c => c.power).ThenBy(c => c.id).ToList();
                    mercenaryIconList = mercenaryIconList
                        .OrderByDescending(c => c.power).ThenBy(c => c.id).ToList();
                    break;
                default:
                    break;
            }
            foreach(var chara in characterIconList) {
                chara.transform.SetAsLastSibling();
                chara.SetInfoTxt(type);
            }
            foreach(var mercenary in mercenaryIconList) {
                mercenary.transform.SetAsLastSibling();
                mercenary.SetInfoTxt(type);
            }
            if(isMercenaryOpen) {
                mercenaryList.gameObject.SetActive(true);
            } else {
                ownCharacterList.gameObject.SetActive(true);
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
            if(index == 0) {
                isMercenaryOpen = false;
            } else {
                isMercenaryOpen = true;
            }
        }
        
        private void AddSelectedCharacter(CharacterSelectIcon origin) {
            if(battleCharacterIdList.Count > maxBattleCharacterNum - cpuIconList.Count - 1) {return;}
            foreach(var chara in characterPanelList.Where(
                chara => chara.playerCharacterID == origin.playerCharacterID &&
                !battleCharacterIdList.Contains(origin.playerCharacterID)
            )) {
                origin.OnSelect(true);
                selectedCharacterList.Add(chara);
                battleCharacterIdList.Add(origin.playerCharacterID);
                SetSelectedStatus(origin.power, origin.hp);
                btnBattle.SetEnable(true);
                btnOK.SetEnable(true);
                TweenA.Add(noticeCharacterSelect, 0.01f, 0).Then(() => {
                    noticeCharacterSelect.SetActive(false);
                });
            }
        }

        private void RemoveSelectedCharacter(uint id) {
            battleCharacterIdList.Remove(id);
            for(int i = 0, l = selectedCharacterList.Count; i < l; i++) {
                if(selectedCharacterList[i].playerCharacterID == id) {
                    selectedCharacterList[i].DisappearCharacter();
                    selectedCharacterList.RemoveAt(i);
                    break;
                }
            }
            if(battleCharacterIdList.Count < 1) {
                btnBattle.SetEnable(false);
                btnOK.SetEnable(false);
                noticeCharacterSelect.SetActive(true);
                TweenA.Add(noticeCharacterSelect, 0.05f, 1).Delay(0.8f);
            }
        }

        private void RemoveAllSelectedCharacter() {
            selectedPower = 0;
            selectedHp = 0;
            SetSelectedStatus(0, 0);
            crntDecaCharacterNum = 0;
            crntMercenaryNum = 0;

            battleCharacterIdList = new List<uint>();
            selectedCharacterList = new List<CharacterSelectPanel>();
            for(int i = 0, l = characterIconList.Count; i < l; i++) {
                characterIconList[i].OnSelect(false);
                characterPanelList[i].InActiveCharacter();
            }
            btnBattle.SetEnable(false);
            btnOK.SetEnable(false);
            noticeCharacterSelect.SetActive(true);
            TweenA.Add(noticeCharacterSelect, 0.05f, 1).Delay(0.8f);
        }

        private void MoveSelectedCharacter(float velocity = 500f) {
            selectedCharacterList = selectedCharacterList
                .OrderBy(c => c.position)
                .ThenBy(c => c.id).ToList();

            for(int i = 0, l = selectedCharacterList.Count; i < l; i++) {
                var n = i;
                selectedCharacterList[i].ChangeOrder(i);
                if(selectedCharacterList[i].gameObject.activeSelf) {
                    selectedCharacterList[i].MoveCharacter(selectedPositionList[i], velocity);
                } else {
                    selectedCharacterList[n].AppearCharacter(selectedPositionList[i], velocity);
                }
            }
        }

        private void SetSelectedStatus(int power, int hp) {
            selectedPower += power;
            selectedHp += hp;

            txtSelectedPower.text = string.Format("{0:#,0}\r", selectedPower);
            txtSelectedHp.text = string.Format("{0:#,0}\r", selectedHp);
            if(selectedHp > 0) {
                TweenS.Add(txtSelectedPower.gameObject, 0.1f, 1.15f).EaseOutExpo()
                    .Then(() => {
                        TweenS.Add(txtSelectedPower.gameObject, 0.2f, 1.0f).EaseInQuart();
                    });
                TweenS.Add(txtSelectedHp.gameObject, 0.1f, 1.15f).EaseOutExpo()
                    .Then(() => {
                        TweenS.Add(txtSelectedHp.gameObject, 0.2f, 1.0f).EaseInQuart();
                    });
            }
        }

        private void SetRequireCharacter(uint[] idList) {
            for(int i = 0; i < idList.Length; i++) {
                foreach(var icon in characterIconList.Where(icon => icon.id == idList[i])) {
                    icon.SetRequire();
                    AddSelectedCharacter(icon);
                }
                foreach(var panel in characterPanelList.Where(panel => panel.id == idList[i])) {
                    panel.SetRequire();
                }
            }
        }

        private void SetCPUCharacter(uint[] idList) {
            for(int i = 0; i < idList.Length; i++) {
                bool haveCharacter = false;
                if(idList[i] < 100) {continue;}
                uint id = uint.Parse(idList[i].ToString().Substring(0, 3));
                foreach(var icon in characterIconList.Where(icon => icon.id == id)) {
                    haveCharacter = true;
                }
                if(!haveCharacter) {
                    CreateCPUCharacter(id);
                }
            }
        }

        private void SetDefaultCharacter(uint[] idList) {
            for(int i = 0; i < idList.Length; i++) {
                foreach(var icon in characterIconList.Where(icon => icon.id == idList[i])) {
                    AddSelectedCharacter(icon);
                }
            }
        }

        private void StartBattle() {
            foreach(var chara in selectedCharacterList) {
                chara.MoveCharacter(new Vector2(4200, 170), 100);
            }
            foreach(var chara in cpuPanelList) {
                chara.MoveCharacter(new Vector2(4200, 170), 100);
            }
            TweenNull.Add(gameObject, 0.8f).Then(() => {
                OnStartBattle.OnNext(battleCharacterIdList.ToArray());
            });
        }

        public void LoadCharacterList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(list => {
                int i = 0;
                foreach (var chara in list) {
                    var charaID = chara.ID;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(charaID).Do(status => {
                        CreateCharacterList(status, charaID);
                        if(i >= list.Length - 1) {
                            SwitchTab(0);
                            SortCharacterList(sortTypeLength - 1);
                            isLoad = true;
                        }
                        i++;
                    }).Subscribe();
                }
            }).Subscribe();
        }

        public void ReloadCharacterList() {
            var gcrm = GlobalContainer.RepositoryManager;
            isLoad = false;
            for(int i = 0, l = characterIconList.Count; i < l; i++) {
                var n = i;
                gcrm.LoadCharacterStatusByPlayerCharacterID(characterIconList[i].playerCharacterID).Do(status => {
                    characterIconList[n].SetInfo(status, characterIconList[n].playerCharacterID);
                    foreach(var panel in characterPanelList.Where(panel => panel.id == status.CharacterID)) {
                        panel.SetCharacterSkin(status.SkinLevel);
                    }
                    if(n <= characterIconList.Count) {
                        isLoad = true;
                    }
                }).Subscribe();
            }
        }

        public void LoadMercenaryList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(list => {
                int i = 0;
                foreach (var chara in list) {
                    var charaID = chara.ID;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(charaID).Do(status => {
                        CreateMercenaryList(status, charaID);
                        if(i == list.Length - 1) {
                            SortCharacterList(sortTypeLength - 1);
                            isLoad = true;
                        }
                        i++;
                    }).Subscribe();
                }
            }).Subscribe();
        }

        public void ReloadMercenaryList() {
            foreach(var icon in mercenaryIconList) {
                Destroy(icon.gameObject);
            }
            foreach(var panel in mercenaryPanelList) {
                Destroy(panel.gameObject);
            }
            mercenaryIconList = new List<CharacterSelectIcon>();
            mercenaryPanelList = new List<CharacterSelectPanel>();
            LoadMercenaryList();
        }
        
        public void SetContinuingStatus(InitialCondition.PlayerCharacterCondition[] characterStatusList) {
            foreach(var character in characterIconList) {
                character.SetContinuingStatus(1, 0);
                if(characterStatusList == null) {continue;}
                foreach(var initChara in characterStatusList) {
                    if(character.playerCharacterID == initChara.PlayerCharacterID) {
                        var hpRatio = (float)initChara.HP / (float)character.hp;
                        var spRatio = (float)initChara.Energy / 1000;
                        character.SetContinuingStatus(hpRatio, spRatio);
                        break;
                    }
                }
            }
        }

        public void SetNewCharacter(uint characterID) {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(characterList => {
                foreach(var character in characterList.Where(character => character.CharacterID == characterID)) {
                    gcrm.LoadCharacterStatusByPlayerCharacterID(character.ID).Do(CharacterStatus => {
                        CreateCharacterList(CharacterStatus, character.ID);
                    }).Subscribe();
                }
            }).Subscribe();
        }

        public uint[] GetPlayerCharacterId(uint[] idList) {
            List<uint> pcIdList = new List<uint>();
            List<CharacterSelectPanel> tempList = new List<CharacterSelectPanel>();
            for(int i = 0, l = idList.Length; i < l; i++) {
                foreach(var chara in characterPanelList.Where(chara => chara.id == idList[i])) {
                    tempList.Add(chara);
                }
            }
            tempList = tempList.OrderBy(c => c.position).ThenBy(c => c.id).ToList();
            for(int i = 0, l = tempList.Count; i < l; i++) {
                pcIdList.Add(tempList[i].playerCharacterID);
            }
            return pcIdList.ToArray();
        }

        public uint[] GetCharacterId(uint[] pcIdList) {
            List<uint> idList = new List<uint>();
            List<CharacterSelectPanel> tempList = new List<CharacterSelectPanel>();
            for(int i = 0, l = pcIdList.Length; i < l; i++) {
                foreach(var chara in characterPanelList.Where(chara => chara.playerCharacterID == pcIdList[i])) {
                    tempList.Add(chara);
                }
            }
            tempList = tempList.OrderBy(c => c.position).ThenBy(c => c.id).ToList();
            for(int i = 0, l = tempList.Count; i < l; i++) {
                idList.Add(tempList[i].id);
            }
            return idList.ToArray();
        }

        public uint[] GetDefaultCharacter(string key) {
            var list = new List<uint>();
            var value = PlayerPrefs.GetString(key);
            if(value != "" && value != null) {
                list = value.Split(new char[]{','}).Select(v => uint.Parse(v)).ToList();
            }
            return list.ToArray();
        }

        public void SaveDefaultCharacter(string key, uint[] pcIdList) {
            var idList = GetCharacterId(pcIdList);
            var list = idList.Select(v => v.ToString()).ToArray();
            var value = string.Join(",", list);
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void OpenPanel(bool isSetting,
            uint[] defaultList = null,
            uint[] requireList = null,
            uint[] cpuList = null
        ) {
            FilterCharacterList(0);
            modal.SetActive(true);
            btnOK.gameObject.SetActive(isSetting);
            btnBattle.gameObject.SetActive(!isSetting);
            tabBtnList[1].gameObject.SetActive(!isSetting);
            if(requireList != null && requireList.Length > 0) {
                SetRequireCharacter(requireList);
            }
            if(cpuList != null && cpuList.Length > 0) {
                SetCPUCharacter(cpuList);
            }
            if(defaultList != null && defaultList.Length > 0) {
                SetDefaultCharacter(defaultList);
            }
            if((defaultList != null && defaultList.Length > 0) ||
               (requireList != null && requireList.Length > 0)) {
                TweenNull.Add(gameObject, 0.2f).Then(() => {MoveSelectedCharacter(0);});
            }
            SortCharacterList(sortTypeLength - 1);
            TweenA.Add(modal, 0.3f, 1).EaseOutCubic();
        }

        public void ClosePanel(bool isBack = true) {
            if(isBack) {
                OnClosePanel.OnNext(null);
            } else {
                List<uint> idList = new List<uint>();
                foreach(var chara in selectedCharacterList) {
                    idList.Add(chara.id);
                }
                OnClosePanel.OnNext(idList.ToArray());
            }
            for(int i = cpuPanelList.Count - 1; i > -1; i--) {
                Destroy(cpuPanelList[i].gameObject);
            }
            cpuPanelList = new List<CharacterSelectPanel>();
            RemoveAllSelectedCharacter();
            TweenA.Add(modal, 0.2f, 0).Then(() => {
                for(int i = cpuIconList.Count - 1; i > -1; i--) {
                    Destroy(cpuIconList[i].gameObject);
                }
                cpuIconList = new List<CharacterSelectIcon>();
                ownCharacterList.verticalNormalizedPosition = 1;
                modal.SetActive(false);
            });
        }
    }
}
