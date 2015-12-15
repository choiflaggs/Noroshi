using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildCreatePanel : MonoBehaviour {
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon btnCreate;
        [SerializeField] InputField inputGuildName;
        [SerializeField] InputField inputIntroduction;
        [SerializeField] Dropdown needLevelSelector;
        [SerializeField] Dropdown statusSelector;
        [SerializeField] Text txtStatusSelector;
        
        private void Start() {
            var gclm = GlobalContainer.LocalizationManager;
            txtStatusSelector.text = gclm.GetText("UI.Noun.OpenGuild");
            statusSelector.options[0].text = gclm.GetText("UI.Noun.OpenGuild");
            statusSelector.options[1].text = gclm.GetText("UI.Noun.CloseGuild");

            if(PlayerInfo.Instance.HaveGem < Noroshi.Core.Game.Guild.Constant.NECESSARY_GEM_TO_CREATE_NORMAL_GUILD) {
                btnCreate.SetEnable(false);
            } else {
                btnCreate.OnClickedBtn.Subscribe(_ => {
                    CreateGuild();
                });
            }

            btnClose.OnClickedBtn.Subscribe(_ => {
                Close();
            });
        }

        private void CreateGuild() {
            var isOpen = statusSelector.value == 0;
            var needLevel = ushort.Parse(needLevelSelector.options[needLevelSelector.value].text);
            var name = inputGuildName.text;
            var intro = inputIntroduction.text;

            Noroshi.Guild.WebApiRequester.Create(isOpen, needLevel, name, intro).Do(data => {
                if(data.Error != null) {

                } else {
                    Application.LoadLevel(Constant.SCENE_GUILD);
                }
            }).Subscribe();
        }
        
        public void Open() {
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
