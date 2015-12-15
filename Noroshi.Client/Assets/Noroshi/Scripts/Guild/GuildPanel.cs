using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildPanel : MonoBehaviour {
        [SerializeField] Image imgGuild;
        [SerializeField] Text txtGuildName;
        [SerializeField] Text txtDescription;
        [SerializeField] Text txtNeedLevel;
        [SerializeField] Text txtCurrentMemberNum;
        [SerializeField] Text txtMaxMemberNum;
        [SerializeField] BtnCommon btnRequest;
        [SerializeField] BtnCommon btnCancelRequest;
        [SerializeField] BtnCommon btnJoin;
        [SerializeField] GameObject nounOpenGuild;
        [SerializeField] GameObject nounCloseGuild;

        public Subject<uint> OnRequest = new Subject<uint>();
        public Subject<uint> OnCancelRequest = new Subject<uint>();
        public Subject<uint> OnJoin = new Subject<uint>();

        private uint guildID;

        private void Start() {
            btnRequest.OnClickedBtn.Subscribe(_ => {
                OnRequest.OnNext(guildID);
            });

            btnCancelRequest.OnClickedBtn.Subscribe(_ => {
                OnCancelRequest.OnNext(guildID);
            });

            btnJoin.OnClickedBtn.Subscribe(_ => {
                OnJoin.OnNext(guildID);
            });
        }

        public void SetPanel(Noroshi.Core.WebApi.Response.Guild.Guild guildData, bool isRequesting = false) {
            guildID = guildData.ID;
            txtGuildName.text = guildData.Name;
            txtDescription.text = guildData.Introduction;
            txtNeedLevel.text = guildData.NecessaryPlayerLevel.ToString();
            txtCurrentMemberNum.text = guildData.MemberNum.ToString();
            txtMaxMemberNum.text = guildData.MaxMemberNum.ToString();
            if(guildData.Category == Noroshi.Core.Game.Guild.GuildCategory.NormalOpen) {
                nounOpenGuild.SetActive(true);
                nounCloseGuild.SetActive(false);
                btnJoin.gameObject.SetActive(true);
            } else {
                nounOpenGuild.SetActive(false);
                nounCloseGuild.SetActive(true);
                btnRequest.gameObject.SetActive(true);
            }
            if(isRequesting) {
                btnJoin.gameObject.SetActive(false);
                btnRequest.gameObject.SetActive(false);
                btnCancelRequest.gameObject.SetActive(true);
            }
        }
    }
}
