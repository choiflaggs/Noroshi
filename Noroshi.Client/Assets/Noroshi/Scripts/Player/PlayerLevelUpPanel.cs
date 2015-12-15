using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniLinq;
using Noroshi.Core.WebApi.Response.Players;
using Noroshi.Core.Game.GameContent;

namespace Noroshi.UI {
    public class PlayerLevelUpPanel : MonoBehaviour {
        [SerializeField] GameObject levelUpPanel;
        [SerializeField] Text txtPreviousLevel;
        [SerializeField] Text txtNewLevel;
        [SerializeField] Text txtPreviousMaxStamina;
        [SerializeField] Text txtNewMaxStamina;
        [SerializeField] GameObject[] unlockContentList;
        [SerializeField] BtnCommon btnOKLevelUp;
        
        public void Init() {
            btnOKLevelUp.OnClickedBtn.Subscribe(_ => {
                TweenA.Add(levelUpPanel, 0.2f, 0).EaseOutCubic().Then(() => {
                    gameObject.SetActive(false);
                });
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            }).AddTo(this);
        }

        public void ShowPanel(AddPlayerExpResult addPlayerExpResult) {
            txtPreviousLevel.text = addPlayerExpResult.PreviousPlayerLevel.ToString();
            txtNewLevel.text = addPlayerExpResult.CurrentPlayerLevel.ToString();
            txtPreviousMaxStamina.text = addPlayerExpResult.PreviousMaxStamina.ToString();
            txtNewMaxStamina.text = addPlayerExpResult.CurrentMaxStamina.ToString();
            var openGameContents = GameContent.BuildMulti(addPlayerExpResult.OpenGameContentIDs).ToArray();
            for(int i = 0, l = unlockContentList.Length; i < l; i++) {
                if(i < openGameContents.Length) {
                    unlockContentList[i].transform.GetChild(0).GetComponent<Text>().text =
                        GlobalContainer.LocalizationManager.GetText(openGameContents[i].TextKey);
                    unlockContentList[i].SetActive(true);
                } else {
                    unlockContentList[i].SetActive(false);
                }
            }
            gameObject.SetActive(true);
            TweenA.Add(levelUpPanel, 0.2f, 1).EaseInCubic();
            SoundController.Instance.PlaySE(SoundController.SEKeys.EVOLUTION);
        }

    }
}
