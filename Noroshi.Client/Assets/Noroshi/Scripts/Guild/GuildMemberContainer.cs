using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class GuildMemberContainer : MonoBehaviour {
        [SerializeField] BtnCommon btnGuildIcon;
        [SerializeField] Text txtGuildName;
        [SerializeField] Text txtRemainGreeting;
        [SerializeField] Text txtMaxGreeting;
        [SerializeField] Text txtRanking;
        [SerializeField] GuildMemberPanel guildMemberPanelPref;
        [SerializeField] GameObject memberListWrapper;
        [SerializeField] AlertModal alertCannotGreet;

        public Subject<int> OnSelectMember = new Subject<int>();
        public Subject<bool> OnSelectGuildIcon = new Subject<bool>();

        private List<GuildMemberPanel> memberList = new List<GuildMemberPanel>();
        private int remainGreetingNum;

        private void Start() {
            btnGuildIcon.OnClickedBtn.Subscribe(_ => {
                OnSelectGuildIcon.OnNext(true);
            });
        }

        public void ChangeLeader(uint id) {
            foreach(var member in memberList) {
                if((uint)member.id == PlayerInfo.Instance.GetPlayerStatus().PlayerID) {
                    member.ChangeLeader(false);
                }
                if((uint)member.id == id) {
                    member.ChangeLeader(true);
                }
            }
        }

        public void NominateOfficer(uint id) {
            foreach(var member in memberList) {
                if((uint)member.id == id) {
                    member.ChangeOfficer(true);
                    break;
                }
            }
        }

        public void ReleaseOfficer(uint id) {
            foreach(var member in memberList) {
                if((uint)member.id == id) {
                    member.ChangeOfficer(false);
                    break;
                }
            }
        }

        public void Expulsion(uint id) {
            foreach(var member in memberList) {
                if((uint)member.id == id) {
                    member.gameObject.SetActive(false);
                    memberList.Remove(member);
                    break;
                }
            }
        }

        public void SetPanel(Noroshi.Core.WebApi.Response.OtherPlayerStatus status, bool canGreet = false) {
            var panel = Instantiate(guildMemberPanelPref);
            panel.transform.SetParent(memberListWrapper.transform);
            panel.transform.localScale = Vector3.one;
            memberList.Add(panel);
            panel.SetPlayerInfo(status);
            if(canGreet) {
                panel.ChangeState(Noroshi.UI.Guild.PanelState.Greeting);
                panel.OnGreeting.Subscribe(data => {
                    if(remainGreetingNum < 1) {
                        alertCannotGreet.OnOpen();
                    } else {
                        Noroshi.Guild.WebApiRequester.Greet(data.PlayerID).Do(res => {
                            panel.ChangeState(Noroshi.UI.Guild.PanelState.None);
                            remainGreetingNum--;
                            txtRemainGreeting.text = remainGreetingNum.ToString();
                        }).Subscribe();
                    }
                });
            }
        }

        public void SetInfo(Noroshi.Core.WebApi.Response.Guild.GetOwnResponse guildData) {
            remainGreetingNum = guildData.MaxGreetingNum - guildData.GreetingNum;
            txtGuildName.text = guildData.Guild.Name;
            txtRanking.text = guildData.Guild.GuildRank.ToString();
            txtRemainGreeting.text = remainGreetingNum.ToString();
            txtMaxGreeting.text = guildData.MaxGreetingNum.ToString();

            for(int i = 0, l = guildData.GuildMembers.Length; i < l; i++) {
                SetPanel(guildData.GuildMembers[i], guildData.GuildMembers[i].CanGreet);
            }
        }
    }
}
