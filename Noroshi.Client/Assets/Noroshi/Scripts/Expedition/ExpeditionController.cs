using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using Noroshi.UI;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Core.WebApi.Response.Battle;

namespace Noroshi.UI {
    public class ExpeditionController : MonoBehaviour {
        [SerializeField] ScrollRect scroller;
        [SerializeField] RectTransform contentWrapper;
        [SerializeField] GameObject treasureListContainer;
        [SerializeField] BtnExpeditionStage[] btnStageList;
        [SerializeField] BtnExpeditionTreasure[] btnTreasureList;
        [SerializeField] GameObject[] roadList;
        [SerializeField] ExpeditionInfo expeditionInfo;
        [SerializeField] TreasureInfo treasureInfo;
        [SerializeField] GameObject getTreasureAlert;
        [SerializeField] GameObject startModal;
        [SerializeField] BtnCommon[] btnSelectLevelList;
        [SerializeField] GameObject restartModal;
        [SerializeField] BtnCommon btnDecideRestart;
        [SerializeField] BtnCommon btnCloseRestart;
        [SerializeField] BtnCommon btnRestart;
        [SerializeField] GameObject guideArrow;
        [SerializeField] AlertModal alertRestart;

        private bool isLoad = false;
        private bool canAcquireReward = false;
        private int currentIndex = 0;
        private int resetNum;
        private uint stageID;
        private uint[] defaultCharacterIdList;
        private InitialCondition.PlayerCharacterCondition[] continuingCharacterList;
        private SkeletonAnimation character;

        private Noroshi.Core.WebApi.Response.Expedition.PlayerExpeditionStage[] stageDataList;

        private void Start() {
            defaultCharacterIdList = BattleCharacterSelect.Instance.GetDefaultCharacter(SaveKeys.DefaultExpeditionBattleCharacter);
            var defaultPlayerCharacterIdList = BattleCharacterSelect.Instance.GetPlayerCharacterId(defaultCharacterIdList).ToList();
            Noroshi.Expedition.WebApiRequester.Get().Do(data => {
                for(int i = 0, l = btnSelectLevelList.Length; i < l; i++) {
                    // 選択式ではなくなったので暫定対応。
                    if(i < 1) {
                        btnSelectLevelList[i].SetEnable(true);
                    } else {
                        btnSelectLevelList[i].SetEnable(false);
                    }
                }
                if(data.PlayerExpedition.IsActive) {
                    stageDataList = data.PlayerExpedition.Stages;
                    resetNum = data.PlayerExpedition.ResetNum;
                    continuingCharacterList = data.PlayerExpedition.PlayerCharacterConditions;
                    SetStage(data.PlayerExpedition.ClearStep.Value, data.PlayerExpedition.CanReceiveReward);
                    foreach(var pcId in defaultPlayerCharacterIdList) {
                        foreach(var character in data.PlayerExpedition.PlayerCharacterConditions) {
                            if(pcId == character.PlayerCharacterID && character.HP <= 0) {
                                defaultPlayerCharacterIdList.Remove(character.PlayerCharacterID);
                                break;
                            }
                        }
                    }
                    defaultCharacterIdList = defaultPlayerCharacterIdList.ToArray();
                } else {
                    expeditionInfo.CloseExpeditionInfo();
                    startModal.SetActive(true);
                }
                isLoad = true;
            }).Subscribe();

            foreach(var btn in btnStageList) {
                btn.OnClickedBtn.Subscribe(SetStageInfo);
                btn.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }
            foreach(var btn in btnTreasureList) {
                btn.OnClickedBtn.Subscribe(GetTreasure);
            }

            expeditionInfo.OnSelectBattle.Subscribe(id => {
                stageID = (uint)id;
                BattleCharacterSelect.Instance.OpenPanel(false, defaultCharacterIdList);
                TweenNull.Add(gameObject, 0.3f).Then(() => {
                    expeditionInfo.gameObject.SetActive(false);
                });
            });

            foreach(var btn in btnSelectLevelList) {
                btn.OnClickedBtn.Subscribe(id => {
                    // 選択式ではなくなったので id を渡さない暫定対応。
                    Noroshi.Expedition.WebApiRequester.Start().Do(data => {
                        stageDataList = data.PlayerExpedition.Stages;
                        resetNum = data.PlayerExpedition.ResetNum;
                        startModal.SetActive(false);
                        defaultCharacterIdList = new uint[]{};
                        continuingCharacterList = data.PlayerExpedition.PlayerCharacterConditions;
                        SetStage(data.PlayerExpedition.ClearStep.Value, data.PlayerExpedition.CanReceiveReward);
                        BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultExpeditionBattleCharacter, new uint[]{});
                        BattleCharacterSelect.Instance.SetContinuingStatus(continuingCharacterList);
                    }).Subscribe();
                });
                btn.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnRestart.OnClickedBtn.Subscribe(_ => {
                if(resetNum > 0) {
                    alertRestart.OnOpen();
                } else {
                    restartModal.SetActive(true);
                }
            });
            btnRestart.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnDecideRestart.OnClickedBtn.Subscribe(_ => {
                Noroshi.Expedition.WebApiRequester.Reset().Subscribe();
                restartModal.SetActive(false);
                startModal.SetActive(true);
            });
            btnDecideRestart.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });

            btnCloseRestart.OnClickedBtn.Subscribe(_ => {
                restartModal.SetActive(false);
            });
            btnCloseRestart.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultExpeditionBattleCharacter, playerCharacterIds);
                BattleScene.Bridge.Transition.TransitToExpeditionBattle(stageID, playerCharacterIds);
            }).AddTo(this);

            BattleCharacterSelect.Instance.OnClosePanel.Subscribe(idList => {
                expeditionInfo.gameObject.SetActive(true);
            }).AddTo(this);

            BattleCharacterSelect.Instance.ReloadCharacterList();
            StartCoroutine("OnLoading");
        }
        
        private IEnumerator OnLoading() {
            while(!isLoad || !BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            BattleCharacterSelect.Instance.SetContinuingStatus(continuingCharacterList);
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.ARENA);
            }
        }

        private void SetStage(uint clearStep, bool isAcquire) {
            int stageDataLength = stageDataList.Length;
            var wrapperSizeDelta = contentWrapper.sizeDelta;

            currentIndex = (int)clearStep;
            canAcquireReward = isAcquire;
            SetStageInfo(currentIndex);

            var chara = Instantiate(
                Resources.Load("UICharacter/" + "105" + "/Character")
            ) as GameObject;
            character = chara.GetComponent<SkeletonAnimation>();
            character.transform.SetParent(contentWrapper.transform);
            character.transform.localScale = new Vector3(-20, 20, 20);
            character.GetComponent<MeshRenderer>().sortingOrder = canAcquireReward ? 2 : 4;

            for(int i = 0, l = btnStageList.Length; i < l; i++) {
                if(i > stageDataLength - 1) {
                    btnStageList[i].gameObject.SetActive(false);
                } else if(i > currentIndex) {
                    btnStageList[i].SetStageState(1);
                } else if(i == currentIndex && !canAcquireReward) {
                    btnStageList[i].SetStageState(2);
                } else {
                    btnStageList[i].SetStageState(3);
                }
            }
            
            for(int i = 0, l = btnTreasureList.Length; i < l; i++) {
                if(i > stageDataLength - 1) {
                    btnTreasureList[i].gameObject.SetActive(false);
                } else if(i < currentIndex) {
                    btnTreasureList[i].gameObject.SetActive(true);
                    btnTreasureList[i].SetTreasureState(true);
                } else {
                    btnTreasureList[i].gameObject.SetActive(true);
                    btnTreasureList[i].SetTreasureState(false);
                }
            }

            for(int i = 0, l = roadList.Length; i < l; i++) {
                if(i > stageDataLength * 2 - 2) {
                    roadList[i].gameObject.SetActive(false);
                } else if(i < currentIndex * 2) {
                    TweenA.Add(roadList[i], 0.01f, 1);
                } else if(i == currentIndex * 2 && canAcquireReward) {
                    TweenA.Add(roadList[i], 0.01f, 1);
                }
            }

            if(currentIndex > stageDataLength - 1) {
                character.gameObject.SetActive(false);
                expeditionInfo.CloseExpeditionInfo();
            } else {
                if(canAcquireReward) {
                    character.transform.localPosition = btnTreasureList[currentIndex].transform.localPosition;
                    expeditionInfo.CloseExpeditionInfo();
                    getTreasureAlert.SetActive(true);
                } else {
                    character.transform.localPosition = btnStageList[currentIndex].transform.localPosition;
                }
            }
            wrapperSizeDelta.x = btnTreasureList[stageDataLength - 1].transform.localPosition.x + 150;
            if(wrapperSizeDelta.x < Constant.SCREEN_BASE_WIDTH) {wrapperSizeDelta.x = Constant.SCREEN_BASE_WIDTH;}
            contentWrapper.sizeDelta = wrapperSizeDelta;

            var initialPosition = (btnStageList[currentIndex].transform.localPosition.x - Constant.SCREEN_BASE_WIDTH / 2) / (wrapperSizeDelta.x - Constant.SCREEN_BASE_WIDTH);
            if(initialPosition < 0) {initialPosition = 0;}
            if(initialPosition > 1) {initialPosition = 1;}
            scroller.horizontalNormalizedPosition = initialPosition;
        }

        private void SetStageInfo(int index) {
            if(index > currentIndex || (index == currentIndex + 1 && canAcquireReward)) {return;}
            var isClear = true;
            if((canAcquireReward && index == currentIndex + 1) || (!canAcquireReward && index == currentIndex)) {
                isClear = false;
            }
            expeditionInfo.OpenExpeditionInfo(stageDataList[index], stageDataList.Length, index, isClear);
        }

        private void GetTreasure(int index) {
            if(canAcquireReward && index == currentIndex) {
                // 冒険ポイントが入り、ゴールド含めて Rewards に含めたため一旦 gold はゼロの暫定対応。
                uint gold = 0;
                treasureInfo.OpenTreasureInfo(gold, stageDataList[currentIndex].Rewards);
                btnTreasureList[currentIndex].SetTreasureState(true);
                getTreasureAlert.SetActive(false);
                currentIndex++;
                canAcquireReward = false;
                btnStageList[currentIndex].SetStageState(2);
                if(currentIndex > stageDataList.Length - 1) {
                    character.gameObject.SetActive(false);
                } else {
                    SetStageInfo(currentIndex);
                    character.state.SetAnimation(0, Constant.ANIM_WALK, true);
                    TweenXY.Add(character.gameObject, 0.8f, btnStageList[currentIndex].transform.localPosition).Then(() => {
                        character.state.SetAnimation(0, Constant.ANIM_IDLE, true);
                    });
                }
                Noroshi.Expedition.WebApiRequester.ReceiveReward().Subscribe();
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            }
        }

        public void OnScroll() {
            treasureListContainer.SetActive(false);
            treasureListContainer.SetActive(true);
        }
    }
}
