using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildMemberPanel : BtnCommon {
        public class MemberData {
            public uint PlayerID;
            public string Name;
        }

        [SerializeField] Image imgPlayer;
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtPlayerLevel;
        [SerializeField] BtnCommon btnGreeting;
        [SerializeField] BtnCommon btnExpulsion;
        [SerializeField] BtnCommon btnNominateLeader;
        [SerializeField] BtnCommon btnNominateOfficer;
        [SerializeField] BtnCommon btnReleaseOfficer;
        [SerializeField] BtnCommon btnApprove;
        [SerializeField] BtnCommon btnReject;
        [SerializeField] GameObject tagLeader;
        [SerializeField] GameObject tagOfficer;

        public Subject<MemberData> OnGreeting = new Subject<MemberData>();
        public Subject<MemberData> OnExpulsion = new Subject<MemberData>();
        public Subject<MemberData> OnNominateLeader = new Subject<MemberData>();
        public Subject<MemberData> OnNominateOfficer = new Subject<MemberData>();
        public Subject<MemberData> OnReleaseOfficer = new Subject<MemberData>();
        public Subject<MemberData> OnApprove = new Subject<MemberData>();
        public Subject<MemberData> OnReject = new Subject<MemberData>();

        private bool isLeader = false;
        private bool isOfficer = false;
        private MemberData memberData = new MemberData();

        private void Start() {
            btnGreeting.OnClickedBtn.Subscribe(_ => {
                OnGreeting.OnNext(memberData);
            });

            btnExpulsion.OnClickedBtn.Subscribe(_ => {
                OnExpulsion.OnNext(memberData);
            });

            btnNominateLeader.OnClickedBtn.Subscribe(_ => {
                OnNominateLeader.OnNext(memberData);
            });

            btnNominateOfficer.OnClickedBtn.Subscribe(_ => {
                OnNominateOfficer.OnNext(memberData);
            });

            btnReleaseOfficer.OnClickedBtn.Subscribe(_ => {
                OnReleaseOfficer.OnNext(memberData);
            });

            btnApprove.OnClickedBtn.Subscribe(_ => {
                OnApprove.OnNext(memberData);
            });

            btnReject.OnClickedBtn.Subscribe(_ => {
                OnReject.OnNext(memberData);
            });
        }

        public void SetPlayerInfo(Noroshi.Core.WebApi.Response.OtherPlayerStatus status) {
            memberData.PlayerID = status.ID;
            memberData.Name = status.Name;
            id = (int)status.ID;
            txtPlayerName.text = status.Name;
            txtPlayerLevel.text = status.Level.ToString();
            if(status.GuildRole == Noroshi.Core.Game.Guild.GuildRole.Leader) {
                isLeader = true;
                tagLeader.SetActive(true);
            } else if(status.GuildRole == Noroshi.Core.Game.Guild.GuildRole.Executive) {
                isOfficer = true;
                tagOfficer.SetActive(true);
            }
        }

        public void ChangeState(Noroshi.UI.Guild.PanelState state) {
            btnGreeting.gameObject.SetActive(false);
            btnExpulsion.gameObject.SetActive(false);
            btnNominateLeader.gameObject.SetActive(false);
            btnNominateOfficer.gameObject.SetActive(false);
            btnReleaseOfficer.gameObject.SetActive(false);
            btnApprove.gameObject.SetActive(false);
            btnReject.gameObject.SetActive(false);

            switch(state) {
                case Noroshi.UI.Guild.PanelState.Greeting:
                    btnGreeting.gameObject.SetActive(true); break;
                case Noroshi.UI.Guild.PanelState.Expulsion:
                    if(!isLeader) {
                        btnExpulsion.gameObject.SetActive(true);
                    }
                    break;
                case Noroshi.UI.Guild.PanelState.NominateLeader:
                    if(!isLeader) {
                        btnNominateLeader.gameObject.SetActive(true);
                    }
                    break;
                case Noroshi.UI.Guild.PanelState.NominateOfficer:
                    if(!isLeader) {
                        if(isOfficer) {
                            btnReleaseOfficer.gameObject.SetActive(true);
                        } else {
                            btnNominateOfficer.gameObject.SetActive(true);
                        }
                    }
                    break;
                case Noroshi.UI.Guild.PanelState.Request: 
                    btnApprove.gameObject.SetActive(true);
                    btnReject.gameObject.SetActive(true);
                    break;
                default: break;
            }
        }

        public void ChangeLeader(bool flag) {
            isLeader = flag;
            if(isLeader) {
                tagLeader.SetActive(true);
                tagOfficer.SetActive(false);
            } else {
                tagLeader.SetActive(false);
            }
        }

        public void ChangeOfficer(bool flag) {
            isOfficer = flag;
            if(isOfficer) {
                tagOfficer.SetActive(true);
                if(btnNominateOfficer.gameObject.activeSelf) {
                    btnNominateOfficer.gameObject.SetActive(false);
                    btnReleaseOfficer.gameObject.SetActive(true);
                }
            } else {
                tagOfficer.SetActive(false);
                if(btnReleaseOfficer.gameObject.activeSelf) {
                    btnNominateOfficer.gameObject.SetActive(true);
                    btnReleaseOfficer.gameObject.SetActive(false);
                }
            }
        }
    }
}
