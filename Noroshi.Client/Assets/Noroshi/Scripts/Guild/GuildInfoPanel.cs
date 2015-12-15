using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildInfoPanel : MonoBehaviour {
        [SerializeField] BtnCommon btnSearch;
        [SerializeField] BtnCommon btnCreateGuild;
        [SerializeField] BtnCommon btnUpdate;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] Text txtGuildID;
        [SerializeField] Text txtGuildName;
        [SerializeField] Text txtCurrentMemberNum;
        [SerializeField] Text txtMaxMemberNum;
        [SerializeField] Text txtRanking;
        [SerializeField] Text txtHighestRanking;
        [SerializeField] Text txtNeedLevel;
        [SerializeField] Text txtGuildIntroduction;
        [SerializeField] GameObject nounBeginner;
        [SerializeField] GameObject nounOpen;
        [SerializeField] GameObject nounClose;
        [SerializeField] InputField inputIntroduction;
        [SerializeField] Text introductionPlaceholder;
        [SerializeField] InputField inputGuildName;
        [SerializeField] Text guildNamePlaceholder;
        [SerializeField] Dropdown needLevelSelector;
        [SerializeField] Text txtNeedLevelSelector;
        [SerializeField] Dropdown statusSelector;
        [SerializeField] Text txtStatusSelector;

        public Subject<bool> OnSelectSearch = new Subject<bool>();
        public Subject<bool> OnSelectCreateGuild = new Subject<bool>();
        
        private void Start() {
            btnSearch.OnClickedBtn.Subscribe(_ => {
                Close();
                OnSelectSearch.OnNext(true);
            });

            btnCreateGuild.OnClickedBtn.Subscribe(_ => {
                Close();
                OnSelectCreateGuild.OnNext(true);
            });

            btnUpdate.OnClickedBtn.Subscribe(_ => {
                var isOpen = statusSelector.value == 0;
                var needLevel = ushort.Parse(needLevelSelector.options[needLevelSelector.value].text);
                var name = inputGuildName.text;
                var intro = inputIntroduction.text;

                Noroshi.Guild.WebApiRequester.Configure(isOpen, needLevel, name, intro).Do(data => {
                    SetGuildInfo(data.Guild);
                    Close();
                }).Subscribe();
            });

            btnClose.OnClickedBtn.Subscribe(_ => {
                Close();
            });
        }

        public void SetGuildInfo(Noroshi.Core.WebApi.Response.Guild.Guild guildData) {
            var gclm = GlobalContainer.LocalizationManager;
            txtGuildID.text = guildData.ID.ToString();
            txtGuildName.text = guildData.Name;
            inputGuildName.text = guildData.Name;
            guildNamePlaceholder.text = guildData.Name;
            txtCurrentMemberNum.text = guildData.MemberNum.ToString();
            txtMaxMemberNum.text = guildData.MaxMemberNum.ToString();
//            txtRanking.text;
//            txtHighestRanking.text;
            txtNeedLevel.text = guildData.NecessaryPlayerLevel.ToString();
            txtNeedLevelSelector.text = guildData.NecessaryPlayerLevel.ToString();
            txtGuildIntroduction.text = guildData.Introduction;
            inputIntroduction.text = guildData.Introduction;
            introductionPlaceholder.text = guildData.Introduction;
            statusSelector.options[0].text = gclm.GetText("UI.Noun.OpenGuild");
            statusSelector.options[1].text = gclm.GetText("UI.Noun.CloseGuild");
            if(guildData.Category == Noroshi.Core.Game.Guild.GuildCategory.Beginner) {
                nounBeginner.SetActive(true);
                nounOpen.SetActive(false);
                nounClose.SetActive(false);
            } else if(guildData.Category == Noroshi.Core.Game.Guild.GuildCategory.NormalOpen) {
                nounBeginner.SetActive(false);
                nounOpen.SetActive(true);
                nounClose.SetActive(false);
                txtStatusSelector.text = gclm.GetText("UI.Noun.OpenGuild");
            } else if(guildData.Category == Noroshi.Core.Game.Guild.GuildCategory.NormalClose) {
                nounBeginner.SetActive(false);
                nounOpen.SetActive(false);
                nounClose.SetActive(true);
                txtStatusSelector.text = gclm.GetText("UI.Noun.CloseGuild");
            }
        }
        
        public void Open(bool isEdit = false) {
            inputIntroduction.gameObject.SetActive(isEdit);
            inputGuildName.gameObject.SetActive(isEdit);
            needLevelSelector.gameObject.SetActive(isEdit);
            statusSelector.gameObject.SetActive(isEdit);

            btnSearch.gameObject.SetActive(!isEdit);
            btnCreateGuild.gameObject.SetActive(!isEdit);
            btnUpdate.gameObject.SetActive(isEdit);

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
