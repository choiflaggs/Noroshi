using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response.Players;
using Noroshi.Core.WebApi.Response.Quest;

namespace Noroshi.UI {
    public class QuestComplete : MonoBehaviour {
        [SerializeField] Text txtQuestName;
        [SerializeField] BtnCommon btnOK;
        [SerializeField] GameObject rewardContainer;
        [SerializeField] GameObject completeRewardExp;
        [SerializeField] GameObject completeRewardItem;

        private AddPlayerExpResult addPlayerExpResult;

        private void Start() {
            btnOK.OnClickedBtn.Subscribe(_ => {
                gameObject.SetActive(false);
                PlayerInfo.Instance.ReceivedQuestComplete(addPlayerExpResult);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        public void OpenQuestResult(ReceiveRewardResponse questData) {
            foreach(Transform rewardItem in rewardContainer.transform) {
                Destroy(rewardItem.gameObject);
            }

            txtQuestName.text = GlobalContainer.LocalizationManager.GetText(questData.Quest.TextKey + ".Name");
            foreach(var reward in questData.Quest.PossessionObjects) {
                if(reward.Category != (byte)Noroshi.Core.Game.Possession.PossessionCategory.Status) {
                    var item = Instantiate(completeRewardItem);
                    item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", reward.ID)
                    );
                    item.transform.GetChild(1).GetComponent<Text>().text = reward.Num.ToString();
                    item.transform.SetParent(rewardContainer.transform);
                    item.transform.localScale = Vector3.one;
                } else {
                    var exp = Instantiate(completeRewardExp);
                    exp.transform.GetChild(1).GetComponent<Text>().text = reward.Num.ToString();
                    exp.transform.SetParent(rewardContainer.transform);
                    exp.transform.localScale = Vector3.one;
                }
            }
            addPlayerExpResult = questData.AddPlayerExpResult;
            gameObject.SetActive(true);
        }
    }
}
