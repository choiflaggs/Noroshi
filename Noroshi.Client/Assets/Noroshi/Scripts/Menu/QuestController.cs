using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response.Quest;

namespace Noroshi.UI {
    public class QuestController : MonoBehaviour {
        [SerializeField] BtnCommon btnOpenDailyQuest;
        [SerializeField] BtnCommon btnOpenQuest;
        [SerializeField] BtnCommon dailyQuestOverlay;
        [SerializeField] BtnCommon questOverlay;
        [SerializeField] GameObject dailyQuestContainer;
        [SerializeField] GameObject dailyQuestWrapper;
        [SerializeField] GameObject questContainer;
        [SerializeField] GameObject questWrapper;
        [SerializeField] QuestPanel questPanel;
        [SerializeField] QuestComplete questComplete;
        [SerializeField] GameObject iconMenuDailyQuestAttention;
        [SerializeField] GameObject iconMenuQuestAttention;
        [SerializeField] GameObject iconDailyQuestAttention;
        [SerializeField] GameObject iconQuestAttention;
        [SerializeField] GameObject noQuestAlert;

        private List<QuestPanel> dailyQuestPanelList;
        private List<QuestPanel> questPanelList;


        private void Start() {
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!PlayerInfo.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            LoadQuestData();
        }

        private void LoadQuestData() {
//            if(PlayerInfo.Instance.GetTutorialStep() >= Noroshi.Core.Game.Player.TutorialStep.ClearStoryStage8) {
//                LoadQuestList();
//                btnOpenQuest.OnClickedBtn.Subscribe(_ => {
//                    OpenQuest();
//                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
//                });
//                questOverlay.OnClickedBtn.Subscribe(_ => {
//                    CloseQuest();
//                    SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
//                });
//            }

            if(PlayerInfo.Instance.GetTutorialStep() >= Noroshi.Core.Game.Player.TutorialStep.ClearStoryStage9) {
                LoadDailyQuestList();
                btnOpenDailyQuest.OnClickedBtn.Subscribe(_ => {
                    OpenDailyQuest();
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });

                dailyQuestOverlay.OnClickedBtn.Subscribe(_ => {
                    CloseDailyQuest();
                    SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
                });
            }
        }

        private void LoadDailyQuestList() {
            var isShowQuestPanel = false;
            dailyQuestPanelList = new List<QuestPanel>();
            foreach(Transform quest in dailyQuestWrapper.transform) {
                Destroy(quest.gameObject);
            }
            Noroshi.Menu.WebApiRequester.DailyList().Do(data => {
                if(data.Quests.Length > 0) {
                    noQuestAlert.SetActive(false);

                    foreach(var quest in data.Quests) {
                        if(!quest.HasAlreadyReceivedReward) {
                            isShowQuestPanel = true;
                            CreateDailyQuestPanel(quest);
                            if(quest.CanReceiveReward) {
                                iconMenuDailyQuestAttention.SetActive(true);
                                iconDailyQuestAttention.SetActive(true);
                            }
                        }
                    }
                    for(int i = dailyQuestPanelList.Count - 1; i > -1; i--) {
                        dailyQuestPanelList[i].CheckClearState();
                    }
                    if(!isShowQuestPanel) {noQuestAlert.SetActive(true);}
                } else {
                    noQuestAlert.SetActive(true);
                }
            }).Subscribe();
        }

        private void LoadQuestList() {
            var isShowQuestPanel = false;
            questPanelList = new List<QuestPanel>();
            foreach(Transform quest in questWrapper.transform) {
                Destroy(quest.gameObject);
            }
            Noroshi.Menu.WebApiRequester.QuestList().Do(data => {
                if(data.Quests.Length > 0) {
                    noQuestAlert.SetActive(false);
                    
                    foreach(var quest in data.Quests) {
                        if(!quest.HasAlreadyReceivedReward) {
                            isShowQuestPanel = true;
                            CreateQuestPanel(quest);
                            if(quest.CanReceiveReward) {
                                iconMenuQuestAttention.SetActive(true);
                                iconQuestAttention.SetActive(true);
                            }
                        }
                    }
                    for(int i = questPanelList.Count - 1; i > -1; i--) {
                        questPanelList[i].CheckClearState();
                    }
                    if(!isShowQuestPanel) {noQuestAlert.SetActive(true);}
                } else {
                    noQuestAlert.SetActive(true);
                }
            }).Subscribe();
        }

        private void CreateDailyQuestPanel(Quest questData) {
            var panel = Instantiate(questPanel);
            panel.transform.SetParent(dailyQuestWrapper.transform);
            panel.transform.localScale = Vector3.one;
            panel.SetQuestInfo(questData);
            dailyQuestPanelList.Add(panel);
            panel.OnQuestComplete.Subscribe(id => {
                Noroshi.Menu.WebApiRequester.ReceiveDailyReward(questData.ID).Do(rewardData => {
                    iconMenuDailyQuestAttention.SetActive(false);
                    iconDailyQuestAttention.SetActive(false);
                    LoadDailyQuestList();
                    questComplete.OpenQuestResult(rewardData);
                }).Subscribe();
            });
        }

        private void CreateQuestPanel(Quest questData) {
            var panel = Instantiate(questPanel);
            panel.transform.SetParent(questWrapper.transform);
            panel.transform.localScale = Vector3.one;
            panel.SetQuestInfo(questData);
            questPanelList.Add(panel);
            panel.OnQuestComplete.Subscribe(id => {
                Noroshi.Menu.WebApiRequester.ReceiveDailyReward(questData.ID).Do(rewardData => {
                    iconMenuQuestAttention.SetActive(false);
                    iconQuestAttention.SetActive(false);
                    LoadQuestList();
                    questComplete.OpenQuestResult(rewardData);
                }).Subscribe();
            });
        }

        private void OpenDailyQuest() {
            dailyQuestOverlay.gameObject.SetActive(true);
            dailyQuestContainer.SetActive(true);
            TweenA.Add(dailyQuestContainer, 0.2f, 1);
        }

        private void OpenQuest() {
            questOverlay.gameObject.SetActive(true);
            questContainer.SetActive(true);
            TweenA.Add(questContainer, 0.2f, 1);
        }

        private void CloseDailyQuest() {
            TweenA.Add(dailyQuestContainer, 0.2f, 0).Then(() => {
                dailyQuestOverlay.gameObject.SetActive(false);
                dailyQuestContainer.SetActive(false);
            });
        }

        private void CloseQuest() {
            TweenA.Add(questContainer, 0.2f, 0).Then(() => {
                questOverlay.gameObject.SetActive(false);
                questContainer.SetActive(false);
            });
        }
    }
}
