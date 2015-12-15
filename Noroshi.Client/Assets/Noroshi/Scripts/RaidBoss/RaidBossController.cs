using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;

namespace Noroshi.UI {
    public class RaidBossController : MonoBehaviour {
        [SerializeField] BtnCommon[] tabScrollerList;
        [SerializeField] JustScroller[] scrollerList;
        [SerializeField] GameObject[] bossPanelWrapperList;
        [SerializeField] GameObject[] bossDetailWrapperList;
        [SerializeField] BossListPanel bossPanelPref;
        [SerializeField] BossDetailPanel bossDetailPref;
        [SerializeField] RewardDetailPanel rewardDetailPanel;
        [SerializeField] BattleDetailPanel battleDetailPanel;
        [SerializeField] GetRewardModal getRewardModal;
        [SerializeField] GameObject alertNotDiscoverBoss;
        [SerializeField] RaidGuildRankPanel raidGuildRankPanel;
        [SerializeField] GuildChatContainer chatContainer;
        [SerializeField] BtnCommon btnChat;
        [SerializeField] BtnCommon btnCloseChat;
        [SerializeField] GameObject chatScroller;
        [SerializeField] GameObject inputFieldContainer;
        [SerializeField] GameObject iconTreasure;

        private int currentTabIndex;
        private uint raidBossID;
        private byte useBP;
        private List<BossDetailPanel> inBattleDetailList = new List<BossDetailPanel>();
        private List<BossDetailPanel> timeUpDetailList = new List<BossDetailPanel>();
        private List<BossDetailPanel> defeatDetailList = new List<BossDetailPanel>();
        private float inputFieldWith;
        private float chatScrollerHeight;
        private float offset = 64;
        private bool isChatOpen = false;
        private bool isLoad = false;

        private void Start() {
            Noroshi.RaidBoss.WebApiRequester.List().Do(data => {
                SetPanel(data.ActiveRaidBosses, true);
                SetPanel(data.RewardUnreceivedRaidBosses, false);
                InitScroller();
                raidGuildRankPanel.SetGuildRank(data.Guild);
                chatContainer.Init(data.Guild, false);
                isLoad = true;
            }).Subscribe();

            for(int i = 0, l = tabScrollerList.Length; i < l; i++) {
                var n = i;
                tabScrollerList[i].OnClickedBtn.Subscribe(SwitchTab);
                tabScrollerList[i].OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            inputFieldWith = inputFieldContainer.GetComponent<RectTransform>().sizeDelta.x;
            chatScrollerHeight = chatScroller.GetComponent<RectTransform>().sizeDelta.y;
            btnChat.OnClickedBtn.Subscribe(_ => {
                if(isChatOpen) {
                    CloseChat();
                } else {
                    OpenChat();
                }
            });
            btnChat.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnCloseChat.OnClickedBtn.Subscribe(_ => {
                CloseChat();
            });
            btnCloseChat.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            PlayerInfo.Instance.OnChangedBP.Subscribe(bp => {
                foreach(var detail in inBattleDetailList) {
                    detail.SetBPButtonState();
                }
            }).AddTo(this);

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                Debug.Log("BossID: " + raidBossID + ", BP: " + useBP);
                BattleScene.Bridge.Transition.TransitToGuildRaidBossBattle(raidBossID, playerCharacterIds, useBP);
            }).AddTo(this);

            SwitchTab(0);

            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad || !BattleCharacterSelect.Instance.isLoad || !PlayerInfo.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void SetPanel(Noroshi.Core.WebApi.Response.RaidBoss.RaidBoss[] bossList, bool isActive) {
            var timespan = (DateTime.UtcNow.ToUniversalTime() - Constant.UNIX_EPOCH).TotalSeconds;
            List<BossDetailPanel> detailList;
            GameObject panelWrapperList;
            GameObject detailWrapperList;
            if(isActive) {
                detailList = inBattleDetailList;
                panelWrapperList = bossPanelWrapperList[0];
                detailWrapperList = bossDetailWrapperList[0];
            } else {
                detailList = defeatDetailList;
                panelWrapperList = bossPanelWrapperList[2];
                detailWrapperList = bossDetailWrapperList[2];
            }

            for(int i = 0, l = bossList.Length; i < l; i++) {
                var list = Instantiate(bossPanelPref);
                var detail = Instantiate(bossDetailPref);
                var rt = detail.GetComponent<RectTransform>();
                list.transform.localScale = Vector2.one;
                detail.transform.localScale = Vector2.one;
                rt.offsetMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);
                if(!bossList[i].IsDefeated && bossList[i].EscapedAt - timespan <= 0) {
                    list.transform.SetParent(bossPanelWrapperList[1].transform);
                    detail.transform.SetParent(bossDetailWrapperList[1].transform);
                    timeUpDetailList.Add(detail);
                } else {
                    list.transform.SetParent(panelWrapperList.transform);
                    detail.transform.SetParent(detailWrapperList.transform);
                    detailList.Add(detail);
                }
                list.transform.localScale = Vector2.one;
                detail.transform.localScale = Vector2.one;
                rt.offsetMin = new Vector2(0, 0);
                rt.offsetMax = new Vector2(0, 0);
                list.SetPanel(bossList[i]);
                detail.SetPanel(bossList[i]);
                SetPanelEvent(detail);
            }
        }

        private void SetPanelEvent(BossDetailPanel detail) {
            detail.OnOpenReward.Subscribe(data => {
                rewardDetailPanel.OpenPanel(data);
            });
            detail.OnOpenDetail.Subscribe(data => {
                battleDetailPanel.OpenPanel(data);
            });
            detail.OnSelectBattle.Subscribe(data => {
                raidBossID = data.GuildRaidBossId;
                useBP = data.UseBP;
                BattleCharacterSelect.Instance.OpenPanel(false);
            });
            detail.OnGetReward.Subscribe(GetReward);
        }

        private void InitScroller() {
            for(int i = 0, l = scrollerList.Length; i < l; i++) {
                var n = i;
                var length = i == 0 ? inBattleDetailList.Count
                    : i == 1 ? timeUpDetailList.Count
                    : defeatDetailList.Count;
                if(length < 1) {
                    if(i == 0) {alertNotDiscoverBoss.SetActive(true);}
                    if(i == 2) {iconTreasure.SetActive(false);}
                    tabScrollerList[i].SetEnable(false);
                }
                scrollerList[i].OnScrollEnd.Subscribe(index => {
                    ChangeDetail(n, index);
                });
                scrollerList[i].Init(length);
            }
        }

        private void SwitchTab(int index) {
            currentTabIndex = index;
            for(int i = 0, l = tabScrollerList.Length; i < l; i++) {
                var isSelect = index == i;
                tabScrollerList[i].SetSelect(isSelect);
                scrollerList[i].gameObject.SetActive(isSelect);
                bossDetailWrapperList[i].SetActive(isSelect);
            }
        }

        private void ChangeDetail(int tabIndex, int index) {
            var list = new List<BossDetailPanel>();
            if(tabIndex == 0) {
                list = inBattleDetailList;
            } else if(tabIndex == 1) {
                list = timeUpDetailList;
            } else {
                list = defeatDetailList;
            }
            for(int i = 0, l = list.Count; i < l; i++) {
                if(i == index) {
                    list[i].gameObject.SetActive(true);
                } else {
                    list[i].gameObject.SetActive(false);
                }
            }
        }

        private void GetReward(uint id) {
            Noroshi.RaidBoss.WebApiRequester.ReceiveReward(id).Do(data => {
                var discoveryRewards = data.DiscoveryRewards;
                var entryRewards = data.EntryRewards;
                Debug.Log("discover: " + discoveryRewards.Length + ", entry: " + entryRewards.Length);
                discoveryRewards.Concat(entryRewards);
                getRewardModal.OpenModal(discoveryRewards);
            }).Subscribe();
        }

        private void OpenChat() {
            TweenX.Add(inputFieldContainer, 0.12f, -offset).Then(() => {
                TweenY.Add(chatScroller, 0.1f, chatScrollerHeight + offset).EaseInCubic().Then(() => {
                    isChatOpen = true;
                });
            });
        }

        private void CloseChat() {
            TweenY.Add(chatScroller, 0.1f, -offset).Then(() => {
                TweenX.Add(inputFieldContainer, 0.12f, -inputFieldWith - offset).EaseInCubic().Then(() => {
                    isChatOpen = false;
                });
            });

        }
    }
}
