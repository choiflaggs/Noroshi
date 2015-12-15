using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response.Quest;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.UI {
    public class QuestPanel : MonoBehaviour,  IPointerClickHandler {
        [SerializeField] Text txtQuestTitle;
        [SerializeField] Text txtQuestDescription;
        [SerializeField] Text txtCurrentState;
        [SerializeField] Text txtThreshold;
        [SerializeField] Image imgQuest;
        [SerializeField] GameObject iconGo;
        [SerializeField] GameObject iconClear;
        [SerializeField] GameObject rewardContainer;
        [SerializeField] GameObject rewardExp;
        [SerializeField] GameObject rewardItem;

        public Subject<bool> OnQuestComplete = new Subject<bool>();

        private string sceneName;
        private bool isClear = false;

        private void SetReward(PossessionObject[] rewards) {
            foreach(var reward in rewards) {
                if(reward.Category != (byte)Noroshi.Core.Game.Possession.PossessionCategory.Status) {
                    var item = Instantiate(rewardItem);
                    item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", reward.ID)
                    );
                    item.transform.GetChild(1).GetComponent<Text>().text = reward.Num.ToString();
                    item.transform.SetParent(rewardContainer.transform);
                    item.transform.localScale = Vector3.one;
                } else {
                    var exp = Instantiate(rewardExp);
                    exp.transform.GetChild(1).GetComponent<Text>().text = reward.Num.ToString();
                    exp.transform.SetParent(rewardContainer.transform);
                    exp.transform.localScale = Vector3.one;
                }
            }
        }

        private void SetSceneName(uint id) {
            switch(id) {
                case 1: sceneName = Constant.SCENE_STORY; break;
                case 2: sceneName = Constant.SCENE_TRIAL; break;
                case 3: sceneName = Constant.SCENE_ARENA; break;
                case 4: sceneName = Constant.SCENE_EXPEDITION; break;
                case 5: sceneName = Constant.SCENE_EXPEDITION; break;
                default: break;
            }
        }

        public void SetQuestInfo(Quest questData) {
            txtQuestTitle.text = GlobalContainer.LocalizationManager.GetText(questData.TextKey + ".Name");
            txtQuestDescription.text = GlobalContainer.LocalizationManager.GetText(questData.TextKey + ".Description");
            txtCurrentState.text = questData.Current.ToString();
            txtThreshold.text = questData.Threshold.ToString();
            imgQuest.sprite = Resources.Load<Sprite>(
                string.Format("Quest/Sprites/img_quest{0}", questData.ID)
            );
            SetReward(questData.PossessionObjects);
            SetSceneName(questData.ID);
            if(questData.CanReceiveReward) {
                isClear = true;
            }
        }

        public void CheckClearState() {
            if(isClear) {
                iconClear.SetActive(true);
                iconGo.SetActive(false);
                transform.SetAsFirstSibling();
            }
        }

        public virtual void OnPointerClick(PointerEventData ped) {
            if(isClear) {
                OnQuestComplete.OnNext(true);
                gameObject.SetActive(false);
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            } else {
                UILoading.Instance.LoadScene(sceneName);
            }
        }
    }
}
