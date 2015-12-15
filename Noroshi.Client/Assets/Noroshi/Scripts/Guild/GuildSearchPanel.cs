using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class GuildSearchPanel : MonoBehaviour {
        [SerializeField] BtnCommon btnClose;
        [SerializeField] GuildPanel guildPanelPref;
        [SerializeField] GameObject guildListWrapper;
        [SerializeField] InputField inputGuildID;
        [SerializeField] BtnCommon btnSearch;
        [SerializeField] AlertModal noGuildAlert;
        [SerializeField] AlertModal alertJoinError;
        [SerializeField] AlertModal alertRequestJoin;

        private List<GuildPanel> guildList = new List<GuildPanel>();
        private List<GuildPanel> searchList = new List<GuildPanel>();
        
        private void Start() {
            Noroshi.Guild.WebApiRequester.GetRecommendedGuilds().Do(data => {
                foreach(var guild in data.Guilds) {
                    var panel = Instantiate(guildPanelPref);
                    panel.transform.SetParent(guildListWrapper.transform);
                    panel.transform.localScale = Vector3.one;
                    panel.SetPanel(guild);
                    SetPanelEvent(panel);
                    guildList.Add(panel);
                }
            }).Subscribe();

            btnSearch.OnClickedBtn.Subscribe(_ => {
                SearchGuild();
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                Close();
            });
        }

        private void SearchGuild() {
            uint id = 0;
            var test = uint.TryParse(inputGuildID.text, out id);
            if(id < 1) {return;}
            for(int i = searchList.Count - 1; i > -1; i--) {
                searchList[i].gameObject.SetActive(false);
                searchList.Remove(searchList[i]);
            }
            foreach(var guild in guildList) {
                guild.gameObject.SetActive(false);
            }
            Noroshi.Guild.WebApiRequester.Get(id).Do(data => {
                if(data.Error != null) {
                    noGuildAlert.OnOpen();
                } else {
                    var panel = Instantiate(guildPanelPref);
                    panel.transform.SetParent(guildListWrapper.transform);
                    panel.transform.localScale = Vector3.one;
                    panel.SetPanel(data.Guild);
                    SetPanelEvent(panel);
                    searchList.Add(panel);
                }
            }).Subscribe();
        }

        private void SetPanelEvent(GuildPanel panel) {
            panel.OnJoin.Subscribe(id => {
                Noroshi.Guild.WebApiRequester.Join(id).Do(data => {
                    if(data.Error != null) {
                        alertJoinError.OnOpen();
                    } else {
                        Application.LoadLevel(Constant.SCENE_GUILD);
                    }
                }).Subscribe();
            });
            panel.OnRequest.Subscribe(id => {
                Noroshi.Guild.WebApiRequester.Request(id).Do(data => {
                    if(data.Error != null) {
                        alertJoinError.OnOpen();
                    } else {
                        alertRequestJoin.OnOpen();
                    }
                }).Subscribe();
            });
        }

        public void Open(Noroshi.Core.WebApi.Response.Guild.Guild requestingGuild) {
            inputGuildID.text = "";
            if(requestingGuild != null) {
                var panel = Instantiate(guildPanelPref);
                panel.transform.SetParent(guildListWrapper.transform);
                panel.transform.localScale = Vector3.one;
                panel.SetPanel(requestingGuild, true);
                panel.OnCancelRequest.Subscribe(id => {
                    Noroshi.Guild.WebApiRequester.CancelRequest().Do(data => {
                        panel.gameObject.SetActive(false);
                    }).Subscribe();
                });
            }
            for(int i = searchList.Count - 1; i > -1; i--) {
                searchList[i].gameObject.SetActive(false);
                searchList.Remove(searchList[i]);
            }
            foreach(var guild in guildList) {
                guild.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1).From(0);
        }

        public void Close() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

    }
}
