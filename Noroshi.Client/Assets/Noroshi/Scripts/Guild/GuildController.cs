using UnityEngine;
using System.Collections;
using UniRx;
using Noroshi.Core.Game.Guild;
using Noroshi.Core.Game.GameContent;

namespace Noroshi.UI {
    public class GuildController : MonoBehaviour {
        [SerializeField] GuildManagementContainer managementContainer;
        [SerializeField] GuildMemberContainer memberContainer;
        [SerializeField] GuildChatContainer chatContainer;
        [SerializeField] GuildInfoPanel guildInfoPanel;
        [SerializeField] GuildSearchPanel guildSearchPanel;
        [SerializeField] GuildPlayerInfoPanel guildPlayerInfoPanel;
        [SerializeField] GuildCreatePanel guildCreatePanel;
        [SerializeField] BtnCommon[] btnTabList;
        [SerializeField] GameObject[] contentList;
        [SerializeField] GameObject receiveGreetingPanel;
        [SerializeField] BtnCommon btnCloseReceiveGreeting;

        private bool isLoad = false;
        private Noroshi.Core.WebApi.Response.Guild.Guild requestingGuild;

        private void Start() {
            foreach(var btn in btnTabList) {
                btn.OnClickedBtn.Subscribe(SwitchTab);
            }

            managementContainer.OnSelectMember.Subscribe(index => {
                guildPlayerInfoPanel.Open();
            });
            managementContainer.OnSelectGuildIcon.Subscribe(_ => {
                guildInfoPanel.Open();
            });
            managementContainer.OnChangeGuildInfo.Subscribe(_ => {
                guildInfoPanel.Open(true);
            });
            managementContainer.OnJoin.Subscribe(memberContainer.SetPanel);
            managementContainer.OnChangeLeader.Subscribe(memberContainer.ChangeLeader);
            managementContainer.OnNominateOfficer.Subscribe(memberContainer.NominateOfficer);
            managementContainer.OnReleaseOfficer.Subscribe(memberContainer.ReleaseOfficer);
            managementContainer.OnExpulsion.Subscribe(memberContainer.Expulsion);

            memberContainer.OnSelectMember.Subscribe(index => {
                guildPlayerInfoPanel.Open();
            });
            memberContainer.OnSelectGuildIcon.Subscribe(_ => {
                guildInfoPanel.Open();
            });

            guildInfoPanel.OnSelectSearch.Subscribe(_ => {
                guildSearchPanel.Open(requestingGuild);
            });
            guildInfoPanel.OnSelectCreateGuild.Subscribe(_ => {
                guildCreatePanel.Open();
            });

            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!PlayerInfo.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            LoadGuildData();
            while(!isLoad) {
                yield return new WaitForEndOfFrame();
            }
            SwitchTab(1);
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void LoadGuildData() {
            var isNormalOpen = false;
            var openGameContents = Core.Game.GameContent.GameContent.BuildOpenGameContentsByPlayerLevel(PlayerInfo.Instance.GetPlayerStatus().Level);
            foreach(var content in openGameContents) {
                if((GameContentID)content.ID == GameContentID.NormalGuild) {
                    isNormalOpen = true;
                    break;
                }
            }
            if(PlayerInfo.Instance.GetPlayerStatus().GuildID != null) {
                Noroshi.Guild.WebApiRequester.GetOwn().Do(data => {
                    if(data.Guild.Category == Noroshi.Core.Game.Guild.GuildCategory.Beginner && isNormalOpen) {
                        Noroshi.Guild.WebApiRequester.JoinAutomatically().Do(_ => {
                            Noroshi.Guild.WebApiRequester.GetOwn().Do(newData => {
                                Init(false, newData);
                            }).Subscribe();
                        }).Subscribe();
                    } else {
                        var isBeginner = data.Guild.Category == Noroshi.Core.Game.Guild.GuildCategory.Beginner;
                        Init(isBeginner, data);
                    }
                }).Subscribe();
            } else {
                Noroshi.Guild.WebApiRequester.JoinBeginnerGuild().Do(data => {
                    Noroshi.Guild.WebApiRequester.GetOwn().Do(newData => {
                        Init(true, newData);
                    }).Subscribe();
                }).Subscribe();
            }
        }

        private void Init(bool isBeginner, Noroshi.Core.WebApi.Response.Guild.GetOwnResponse guildData) {
            if(PlayerInfo.Instance.GetPlayerStatus().GuildRole != GuildRole.Leader
               && PlayerInfo.Instance.GetPlayerStatus().GuildRole != GuildRole.Executive) {
                btnTabList[0].gameObject.SetActive(false);
            } else {
                managementContainer.SetState();
            }
            if(guildData.UnconfirmedGreetedNum > 0) {
                SetReceiveGreeting();
            }
            if(guildData.RequestingGuild != null) {
                requestingGuild = guildData.RequestingGuild;
            }
            memberContainer.SetInfo(guildData);
            chatContainer.Init(guildData.Guild, isBeginner);
            managementContainer.SetInfo(guildData);
            guildInfoPanel.SetGuildInfo(guildData.Guild);
            isLoad = true;
        }

        private void SetReceiveGreeting() {
            btnCloseReceiveGreeting.OnClickedBtn.Subscribe(_ => {
                Noroshi.Guild.WebApiRequester.ReceiveGreetedReward().Do(data => {
                    TweenA.Add(receiveGreetingPanel, 0.1f, 0).Then(() => {
                        receiveGreetingPanel.SetActive(false);
                    });
                }).Subscribe();
            });
            receiveGreetingPanel.SetActive(true);
            TweenA.Add(receiveGreetingPanel, 0.1f, 1).From(0);
        }

        private void SwitchTab(int index) {
            for(int i = 0, l = contentList.Length; i < l; i++) {
                if(i == index) {
                    btnTabList[i].SetSelect(true);
                    contentList[i].SetActive(true);
                } else {
                    btnTabList[i].SetSelect(false);
                    contentList[i].SetActive(false);
                }
            }
        }

    }
}
