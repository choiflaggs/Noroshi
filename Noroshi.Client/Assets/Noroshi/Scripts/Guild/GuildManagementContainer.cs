using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.UI {
    public class GuildManagementContainer : MonoBehaviour {
        [SerializeField] BtnCommon btnGuildIcon;
        [SerializeField] Text txtGuildName;
        [SerializeField] BtnCommon btnApproveJoin;
        [SerializeField] BtnCommon btnExpulsion;
        [SerializeField] BtnCommon btnChangeLeader;
        [SerializeField] BtnCommon btnChangeOfficer;
        [SerializeField] BtnCommon btnChangeGuildInfo;
        [SerializeField] GameObject memberListScroller;
        [SerializeField] GameObject requesterListScroller;
        [SerializeField] GuildMemberPanel guildMemberPanelPref;
        [SerializeField] GameObject memberListWrapper;
        [SerializeField] GameObject requesterListWrapper;
        [SerializeField] GameObject alertNoRequest;
        [SerializeField] GuildConfirmPanel nominateLeaderPanel;
        [SerializeField] GuildConfirmPanel nominateOfficerPanel;
        [SerializeField] GuildConfirmPanel releaseOfficerPanel;
        [SerializeField] GuildConfirmPanel expulsionPanel;

        public Subject<int> OnSelectMember = new Subject<int>();
        public Subject<bool> OnSelectGuildIcon = new Subject<bool>();
        public Subject<bool> OnChangeGuildInfo = new Subject<bool>();
        public Subject<OtherPlayerStatus> OnJoin = new Subject<OtherPlayerStatus>();
        public Subject<uint> OnChangeLeader = new Subject<uint>();
        public Subject<uint> OnNominateOfficer = new Subject<uint>();
        public Subject<uint> OnReleaseOfficer = new Subject<uint>();
        public Subject<uint> OnExpulsion = new Subject<uint>();

        private List<GuildMemberPanel> memberList = new List<GuildMemberPanel>();
        private List<GuildMemberPanel> requesterList = new List<GuildMemberPanel>();
        private uint targetPlayerID;
        
        private void Start() {
            btnGuildIcon.OnClickedBtn.Subscribe(_ => {
                OnSelectGuildIcon.OnNext(true);
            });

            btnApproveJoin.OnClickedBtn.Subscribe(_ => {
                memberListScroller.SetActive(false);
                requesterListScroller.SetActive(true);
            });

            btnExpulsion.OnClickedBtn.Subscribe(_ => {
                memberListScroller.SetActive(true);
                requesterListScroller.SetActive(false);
                foreach(var member in memberList) {
                    member.ChangeState(Noroshi.UI.Guild.PanelState.Expulsion);
                }
            });

            btnChangeLeader.OnClickedBtn.Subscribe(_ => {
                memberListScroller.SetActive(true);
                requesterListScroller.SetActive(false);
                foreach(var member in memberList) {
                    member.ChangeState(Noroshi.UI.Guild.PanelState.NominateLeader);
                }
            });

            btnChangeOfficer.OnClickedBtn.Subscribe(_ => {
                memberListScroller.SetActive(true);
                requesterListScroller.SetActive(false);
                foreach(var member in memberList) {
                    member.ChangeState(Noroshi.UI.Guild.PanelState.NominateOfficer);
                }
            });

            btnChangeGuildInfo.OnClickedBtn.Subscribe(_ => {
                OnChangeGuildInfo.OnNext(true);
            });

            nominateLeaderPanel.OnDecide.Subscribe(_ => {
                Noroshi.Guild.WebApiRequester.ChangeLeader(targetPlayerID).Do(data => {
                    OnChangeLeader.OnNext(targetPlayerID);
                    foreach(var member in memberList) {
                        if((uint)member.id == PlayerInfo.Instance.GetPlayerStatus().PlayerID) {
                            member.ChangeLeader(false);
                        }
                        if((uint)member.id == targetPlayerID) {
                            member.ChangeLeader(true);
                        }
                    }
                }).Subscribe();
            });

            nominateOfficerPanel.OnDecide.Subscribe(_ => {
                Noroshi.Guild.WebApiRequester.AddExecutiveRole(targetPlayerID).Do(data => {
                    OnNominateOfficer.OnNext(targetPlayerID);
                    foreach(var member in memberList) {
                        if((uint)member.id == targetPlayerID) {
                            member.ChangeOfficer(true);
                            break;
                        }
                    }
                }).Subscribe();
            });

            releaseOfficerPanel.OnDecide.Subscribe(_ => {
                Noroshi.Guild.WebApiRequester.RemoveExecutiveRole(targetPlayerID).Do(data => {
                    OnReleaseOfficer.OnNext(targetPlayerID);
                    foreach(var member in memberList) {
                        if((uint)member.id == targetPlayerID) {
                            member.ChangeOfficer(false);
                            break;
                        }
                    }
                }).Subscribe();
            });

            expulsionPanel.OnDecide.Subscribe(_ => {
                Noroshi.Guild.WebApiRequester.LayOff(targetPlayerID).Do(data => {
                    OnExpulsion.OnNext(targetPlayerID);
                    foreach(var member in memberList) {
                        if((uint)member.id == targetPlayerID) {
                            member.gameObject.SetActive(false);
                            break;
                        }
                    }
                }).Subscribe();
            });
        }

        private void SetMemberPanel(OtherPlayerStatus status) {
            var panel = Instantiate(guildMemberPanelPref);
            panel.transform.SetParent(memberListWrapper.transform);
            panel.transform.localScale = Vector3.one;
            memberList.Add(panel);
            panel.SetPlayerInfo(status);
            SetMemberPanelEvent(panel);
        }

        private void SetMemberPanelEvent(GuildMemberPanel panel) {
            panel.OnNominateLeader.Subscribe(data => {
                targetPlayerID = data.PlayerID;
                nominateLeaderPanel.OpenPanel(data.Name);
            });
            panel.OnNominateOfficer.Subscribe(data => {
                targetPlayerID = data.PlayerID;
                nominateOfficerPanel.OpenPanel(data.Name);
            });
            panel.OnReleaseOfficer.Subscribe(data => {
                targetPlayerID = data.PlayerID;
                releaseOfficerPanel.OpenPanel(data.Name);
            });
            panel.OnExpulsion.Subscribe(data => {
                targetPlayerID = data.PlayerID;
                expulsionPanel.OpenPanel(data.Name);
            });
        }

        private void SetRequesterEvent(GuildMemberPanel panel) {
            panel.OnApprove.Subscribe(data => {
                Noroshi.Guild.WebApiRequester.AcceptRequest(data.PlayerID).Do(res => {
                    OnJoin.OnNext(res.Requester);
                    SetMemberPanel(res.Requester);
                    panel.gameObject.SetActive(false);
                }).Subscribe();
            });
            panel.OnReject.Subscribe(data => {
                Noroshi.Guild.WebApiRequester.RejectRequest(data.PlayerID).Do(res => {
                    panel.gameObject.SetActive(false);
                }).Subscribe();
            });
        }

        public void SetInfo(Noroshi.Core.WebApi.Response.Guild.GetOwnResponse guildData) {
            txtGuildName.text = guildData.Guild.Name;
            for(int i = 0, l = guildData.GuildMembers.Length; i < l; i++) {
                SetMemberPanel(guildData.GuildMembers[i]);
            }
            for(int i = 0, l = guildData.Requests.Length; i < l; i++) {
                var panel = Instantiate(guildMemberPanelPref);
                panel.transform.SetParent(requesterListWrapper.transform);
                panel.transform.localScale = Vector3.one;
                requesterList.Add(panel);
                panel.SetPlayerInfo(guildData.Requests[i].Requester);
                panel.ChangeState(Noroshi.UI.Guild.PanelState.Request);
                SetRequesterEvent(panel);
            }
            if(guildData.Requests.Length < 1) {
                alertNoRequest.SetActive(true);
            }
        }

        public void SetState() {
            if(PlayerInfo.Instance.GetPlayerStatus().GuildRole == Noroshi.Core.Game.Guild.GuildRole.Executive) {
                btnExpulsion.SetEnable(false);
                btnChangeLeader.SetEnable(false);
                btnChangeOfficer.SetEnable(false);
                btnChangeGuildInfo.SetEnable(false);
            }
        }
    }
}
