using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.CharacterList;
using Noroshi.Core.Game.Player;

namespace Noroshi.UI {
    public class TutorialController : MonoBehaviour {
        [SerializeField] Canvas canvas;
        [SerializeField] GameObject pointer;
        [SerializeField] GameObject descriptionContainer;
        [SerializeField] Text txtDescription;

        private bool haveCanvas;
        private int defaultOrder;

        protected CompositeDisposable disposables = new CompositeDisposable();
        protected CompositeDisposable panelDisposables = new CompositeDisposable();
        
        private void SetFocus(GameObject obj, float offsetX = 0, float offsetY = 0) {
            Canvas objCanvas = obj.GetComponent<Canvas>();
            var objPosition = obj.transform.position;

            if(objCanvas == null) {
                haveCanvas = false;
                obj.AddComponent<GraphicRaycaster>();
                objCanvas = obj.GetComponent<Canvas>();
            } else {
                haveCanvas = true;
                defaultOrder = objCanvas.sortingOrder;
            }
            objCanvas.overrideSorting = true;
            objCanvas.overridePixelPerfect = true;
            objCanvas.pixelPerfect = true;
            objCanvas.sortingOrder = 100;
            objPosition.x += offsetX;
            objPosition.y += offsetY;
            pointer.transform.position = objPosition;
            pointer.SetActive(true);
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void RemoveFocus(GameObject obj) {
            if(haveCanvas) {
                obj.GetComponent<Canvas>().sortingOrder = defaultOrder;
            } else {
                obj.GetComponent<Canvas>().overrideSorting = false;
            }
        }

        private IEnumerator OnCheckingGetCharacter(int index, uint characterID) {
            while(!BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            gameObject.SetActive(false);
            var pcID = BattleCharacterSelect.Instance.GetPlayerCharacterId(new uint[]{characterID});
            if(pcID.Length < 1) {
                var getModal = Instantiate(Resources.Load<GetCharacterModal>("UI/GetCharacterModal"));
                getModal.OpenModal(characterID);
                if(index == 7) {
                    TweenNull.Add(getModal.gameObject, 0.2f).Then(() => {
                        var btnClose = GameObject.Find("BtnCloseGetCharacter");
                        btnClose.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                            gameObject.SetActive(true);
                            SetTutorial7();
                        }).AddTo(disposables);
                    });
                }
            } else {
                if(index == 7) {
                    gameObject.SetActive(true);
                    SetTutorial7();
                }
            }
        }

        private IEnumerator OnCheckingCharacterPanel(int index, uint characterID) {
            var characterList = GameObject.FindObjectOfType<CharacterListViewModel>();
            while(!characterList.CheckCharacterCreated()) {
                yield return new WaitForEndOfFrame();
            }
            var characterPanelList = GameObject.FindObjectsOfType<CharacterPanel>();
            var scroller = GameObject.Find("AcquiredScrollView").GetComponent<ScrollRect>();
            var content = GameObject.Find("AcquiredCharacterListContainer").GetComponent<RectTransform>();
            foreach(var panel in characterPanelList) {
                if(panel.charaData.characterID == characterID) {
                    scroller.enabled = false;
                    var diffY = -panel.transform.localPosition.y - 108;
                    float initialPosition = 1 - (diffY / (content.sizeDelta.y - 450));
                    if(initialPosition < 0) {initialPosition = 0;}
                    if(initialPosition > 1) {initialPosition = 1;}
                    scroller.verticalNormalizedPosition = initialPosition;
                    SetFocus(panel.gameObject);
                    panel.OnOpenDetail.Subscribe(_ => {
                        RemoveFocus(panel.gameObject);
                        scroller.enabled = true;
                        if(index == 2) {
                            SetCharacterTutorial2();
                        } else if(index == 3) {
                            SetCharacterTutorial3();
                        } else if(index == 4) {
                            SetCharacterTutorial4();
                        } else if(index == 6) {
                            SetCharacterTutorial6();
                        } else if(index == 10) {
                            SetCharacterTutorial10();
                        }
                        DisposePanel();
                    }).AddTo(panelDisposables);
                    break;
                }
            }
        }

        private void StartGetCharacterTutorial(int index, uint characterID) {
            StartCoroutine(OnCheckingGetCharacter(index, characterID));
        }

        private void StartCharacterTutorial(int index, uint characterID) {
            if(Application.loadedLevelName != Constant.SCENE_CHARACTER_LIST) {
                var btnMenu = GameObject.Find("BtnMenu");
                if(btnMenu == null) {
                    gameObject.SetActive(false);
                    return;
                }
                TweenNull.Add(gameObject, 0.1f).Then(() => {
                    SetFocus(btnMenu, -0.6f, -0.25f);
                });
                btnMenu.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                    pointer.SetActive(false);
                    RemoveFocus(btnMenu);
                    TweenNull.Add(gameObject, 0.4f).Then(() => {
                        var btnMenu1 = GameObject.Find("Menu1");
                        SetFocus(btnMenu1, 0, 0.1f);
                    });
                }).AddTo(disposables);
                if(index == 2) {
                    txtDescription.text = "アイテムを装備してキャラクターを強化しよう";
                } else if(index == 3) {
                    txtDescription.text = "スキルレベルを上げてキャラクターを強化しよう";
                } else if(index == 4) {
                    txtDescription.text = "キャラクターを昇格させよう";
                } else if(index == 6) {
                    txtDescription.text = "成長剤を使ってキャラクターを強化しよう";
                } else if(index == 10) {
                    txtDescription.text = "召喚石を獲得しました\nキャラクターを進化させよう";
                }
                descriptionContainer.SetActive(true);
            } else {
                StartCoroutine(OnCheckingCharacterPanel(index, characterID));
            }
        }

        private void SetCharacterTutorial2() {
            var equipList = GameObject.Find("EquipList");
            for(int i = 0, l = equipList.transform.childCount - 1; i < l; i++) {
                var equip = equipList.transform.GetChild(i).GetComponent<CharacterList.BtnEquip>();
                if(equip.equipState == -2) {
                    SetFocus(equip.gameObject);
                    equip.OnClickedBtn.Subscribe(_ => {
                        var btnEquip = GameObject.Find("BtnEquip").GetComponent<BtnCommon>();
                        RemoveFocus(equip.gameObject);
                        SetFocus(btnEquip.gameObject, 0, 0.2f);
                        btnEquip.OnClickedBtn.Subscribe(__ => {
                            RemoveFocus(btnEquip.gameObject);
                            gameObject.SetActive(false);
                            Dispose();
                        }).AddTo(disposables);
                    }).AddTo(disposables);
                    break;
                }
            }
        }

        private void SetCharacterTutorial3() {
            var tabSkill = GameObject.Find("TabSkill");
            SetFocus(tabSkill, -0.1f, 0.3f);
            tabSkill.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                var btnSkillUp = GameObject.Find("BtnSkillUp").GetComponent<BtnCommon>();
                var scroller = GameObject.Find("SkillUpScroller");
                scroller.GetComponent<ScrollRect>().enabled = false;
                RemoveFocus(tabSkill);
                SetFocus(scroller, -3.2f, 1.3f);
                btnSkillUp.OnClickedBtn.Subscribe(__ => {
                    RemoveFocus(scroller);
                    scroller.GetComponent<ScrollRect>().enabled = true;
                    gameObject.SetActive(false);
                    Dispose();
                }).AddTo(disposables);
            }).AddTo(disposables);
        }

        private void SetCharacterTutorial4() {
            var equipList = GameObject.Find("EquipList");
            var btnRaisePromotion = equipList.transform.Find("BtnRaisePromotionLv");
            var unequipIndex = 0;
            var unequipList = new List<CharacterList.BtnEquip>();
            BtnCommon btnEquip = null;

            for(int i = 0, l = equipList.transform.childCount - 1; i < l; i++) {
                var equip = equipList.transform.GetChild(i).GetComponent<CharacterList.BtnEquip>();
                if(equip.equipState == -2) {
                    unequipList.Add(equip);
                    equip.OnClickedBtn.Subscribe(_ => {
                        if(btnEquip == null) {
                            btnEquip = GameObject.Find("BtnEquip").GetComponent<BtnCommon>();
                            btnEquip.OnClickedBtn.Subscribe(__ => {
                                unequipIndex++;
                                RemoveFocus(btnEquip.gameObject);
                                if(unequipIndex < unequipList.Count) {
                                    SetFocus(unequipList[unequipIndex].gameObject);
                                } else {
                                    btnRaisePromotion.gameObject.SetActive(true);
                                    SetFocus(btnRaisePromotion.gameObject);
                                }
                            }).AddTo(disposables);
                        }
                        RemoveFocus(equip.gameObject);
                        SetFocus(btnEquip.gameObject);
                    }).AddTo(disposables);
                }
            }
            btnRaisePromotion.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                RemoveFocus(btnRaisePromotion.gameObject);
                TweenNull.Add(gameObject, 0.25f).Then(() => {
                    var btnDecidePromotion = GameObject.Find("BtnDecidePromotion");
                    SetFocus(btnDecidePromotion);
                    btnDecidePromotion.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(__ => {
                        RemoveFocus(btnDecidePromotion);
                        gameObject.SetActive(false);
                        Dispose();
                    }).AddTo(disposables);
                });
            }).AddTo(disposables);

            if(unequipList.Count > 0) {
                SetFocus(unequipList[0].gameObject);
            } else {
                SetFocus(btnRaisePromotion.gameObject);
            }
        }

        private void SetCharacterTutorial6() {
            var btnExpUp = GameObject.Find("BtnExpUp");
            SetFocus(btnExpUp);
            btnExpUp.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                TweenNull.Add(gameObject, 0.25f).Then(() => {
                    var expUpItem = GameObject.Find("ExpUpItem1");
                    RemoveFocus(btnExpUp);
                    SetFocus(expUpItem);
                    expUpItem.GetComponent<CharacterList.ExpUpItem>().OnPanelClick.Subscribe(__ => {
                        RemoveFocus(expUpItem);
                        gameObject.SetActive(false);
                        Dispose();
                    }).AddTo(disposables);
                });
            }).AddTo(disposables);
        }

        private void SetTutorial7() {
            if(Application.loadedLevelName == Constant.SCENE_MAIN) {
                TweenNull.Add(gameObject, 0.2f).Then(() => {
                    var gachaBtn = GameObject.Find("GachaButton");
                    SetFocus(gachaBtn);
                });
            } else if(Application.loadedLevelName == Constant.SCENE_GACHA) {
                var panel = GameObject.Find("GachaPanel2");
                var btnTutorialGacha = panel.transform.Find("BtnTutorialGacha");
                var btnGold = GameObject.Find("BtnGachaGold");
                btnTutorialGacha.gameObject.SetActive(true);
                btnGold.SetActive(false);
                TweenNull.Add(gameObject, 0.2f).Then(() => {
                    SetFocus(btnTutorialGacha.gameObject);
                });
                btnTutorialGacha.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                    gameObject.SetActive(false);
                }).AddTo(disposables);
            } else if(Application.loadedLevelName == Constant.SCENE_STORY) {
                StartCoroutine("CheckStoryLoaded");
                txtDescription.text = "ガチャをひいてみよう";
                descriptionContainer.SetActive(true);
            }
        }

        private IEnumerator CheckStoryLoaded() {
            while(!UILoading.Instance.GetLoadingEnd()) {
                yield return new WaitForEndOfFrame();
            }
            TweenNull.Add(gameObject, 0.1f).Then(() => {
                var btnToMain = GameObject.Find("BtnBackToMain");
                if(btnToMain == null) {
                    var btnBack = GameObject.Find("BtnBackToPartSelect");
                    SetFocus(btnBack, 0.6f, -0.4f);
                    btnBack.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                        TweenNull.Add(gameObject, 0.2f).Then(() => {
                            btnToMain = GameObject.Find("BtnBackToMain");
                            RemoveFocus(btnBack);
                            SetFocus(btnToMain, 0.6f, -0.4f);
                        });
                    }).AddTo(disposables);
                } else {
                    SetFocus(btnToMain, 0.6f, -0.4f);
                }
            });
        }
        
        private void StartTutorial8() {
            var btnMenu = GameObject.Find("BtnMenu");
            if(btnMenu == null) {
                gameObject.SetActive(false);
                return;
            }
            TweenNull.Add(gameObject, 0.1f).Then(() => {
                SetFocus(btnMenu, -0.6f, -0.25f);
            });
            txtDescription.text = "依頼を達成しよう";
            descriptionContainer.SetActive(true);
            btnMenu.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                pointer.SetActive(false);
                RemoveFocus(btnMenu);
                TweenNull.Add(gameObject, 0.4f).Then(() => {
                    var btnMenu4 = GameObject.Find("Menu4");
                    SetFocus(btnMenu4, 0, 0.1f);
                    btnMenu4.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(__ => {
                        descriptionContainer.SetActive(false);
                        RemoveFocus(btnMenu4);
                        btnMenu4.SetActive(false);
                        TweenNull.Add(gameObject, 0.01f).Then(() => {
                            btnMenu4.SetActive(true);
                            gameObject.SetActive(false);
                        });
                        Dispose();
                    }).AddTo(disposables);
                });
            }).AddTo(disposables);
        }

        private void StartTutorial9() {
            var btnMenu = GameObject.Find("BtnMenu");
            if(PlayerPrefs.GetInt(SaveKeys.OpenDailyQuest) < 1 || btnMenu == null) {
                gameObject.SetActive(false);
                return;
            }
            TweenNull.Add(gameObject, 0.1f).Then(() => {
                SetFocus(btnMenu, -0.6f, -0.25f);
            });
            txtDescription.text = "本日の依頼を達成しよう";
            descriptionContainer.SetActive(true);
            btnMenu.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                pointer.SetActive(false);
                RemoveFocus(btnMenu);
                TweenNull.Add(gameObject, 0.4f).Then(() => {
                    var btnMenu5 = GameObject.Find("Menu5");
                    SetFocus(btnMenu5, 0, 0.1f);
                    btnMenu5.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(__ => {
                        descriptionContainer.SetActive(false);
                        RemoveFocus(btnMenu5);
                        btnMenu5.SetActive(false);

                        TweenNull.Add(gameObject, 0.01f).Then(() => {
                            var wrapper = GameObject.Find("DailyQuestWrapper");
                            var panel = wrapper.transform.GetChild(0);
                            if(panel != null) {
                                var isClear = panel.transform.Find("BtnClear");
                                if(isClear.gameObject.activeSelf) {
                                    SetFocus(panel.gameObject, 0, 0.1f);
                                    panel.GetComponent<QuestPanel>().OnQuestComplete.Subscribe(___ => {
                                        gameObject.SetActive(false);
                                    });
                                } else {
                                    gameObject.SetActive(false);
                                }
                            } else {
                                gameObject.SetActive(false);
                            }
                            btnMenu5.SetActive(true);
                        });
                        PlayerPrefs.SetInt(SaveKeys.OpenDailyQuest, 1);
                        PlayerPrefs.Save();
                        Dispose();
                    }).AddTo(disposables);
                });
            }).AddTo(disposables);
        }

        private void SetCharacterTutorial10() {
            var btnEvolution = GameObject.Find("BtnRaiseEvolutionLv");
            SetFocus(btnEvolution);
            btnEvolution.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(_ => {
                RemoveFocus(btnEvolution);
                TweenNull.Add(gameObject, 0.4f).Then(() => {
                    var btnDecideEvolution = GameObject.Find("BtnDecideEvolution");
                    SetFocus(btnDecideEvolution);
                    btnDecideEvolution.GetComponent<BtnCommon>().OnClickedBtn.Subscribe(__ => {
                        RemoveFocus(btnDecideEvolution);
                        gameObject.SetActive(false);
                        Dispose();
                    }).AddTo(disposables);
                });
            }).AddTo(disposables);
        }
        
        private void Dispose() {
            disposables.Dispose();
        }

        private void DisposePanel() {
            panelDisposables.Dispose();
        }

        public void Init(TutorialStep step) {
            canvas.worldCamera = Camera.main;

            if(step == TutorialStep.ClearStoryStage1) {
                StartGetCharacterTutorial(1, 101);
            } else if(step < TutorialStep.EquipGear) {
                StartCharacterTutorial(2, 105);
            } else if(step < TutorialStep.ActionLevelUP) {
                StartCharacterTutorial(3, 105);
            } else if(step < TutorialStep.PromotionLevelUP) {
                StartCharacterTutorial(4, 105);
            } else if(step == TutorialStep.ClearStoryStage5) {
                StartGetCharacterTutorial(5, 303);
            } else if(step < TutorialStep.ConsumeDrug) {
                StartCharacterTutorial(6, 303);
            } else if(step < TutorialStep.LotGacha) {
                StartGetCharacterTutorial(7, 206);
            } else if(step < TutorialStep.ReceiveQuestReward) {
                StartTutorial8();
            } else if(step < TutorialStep.ReceiveDailyQuestReward) {
                StartTutorial9();
            } else if(step < TutorialStep.EvolutionLevelUP) {
                StartCharacterTutorial(10, 206);
            }
        }
    }
}
