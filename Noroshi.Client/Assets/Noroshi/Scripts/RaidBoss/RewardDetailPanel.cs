using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.UI {
    public class RewardDetailPanel : MonoBehaviour {
        [SerializeField] BtnCommon[] tabList;
        [SerializeField] GameObject[] contentList;
        [SerializeField] BtnCommon btnOverlay;
        [SerializeField] GameObject[] entryRewardList;
        [SerializeField] GameObject[] discoverRewardList;
        [SerializeField] GameObject[] friendShipRewardList;
        [SerializeField] Image[] imgEntryRewardList;
        [SerializeField] Image[] imgDiscoverRewardList;
        [SerializeField] Image[] imgFriendShipRewardList;
        [SerializeField] Text[] txtEntryRewardList;
        [SerializeField] Text[] txtDiscoverRewardList;
        [SerializeField] Text[] txtFriendShipRewardList;

        private void Start() {
            for(int i = 0, l = tabList.Length; i < l; i++) {
                var n = i;
                tabList[i].OnClickedBtn.Subscribe(SwitchTab);
            }

            btnOverlay.OnClickedBtn.Subscribe(_ => {
                ClosePanel();
            });
        }

        private void SwitchTab(int index) {
            for(int i = 0, l = contentList.Length; i < l; i++) {
                var isSelect = index == i;
                tabList[i].SetSelect(isSelect);
                contentList[i].SetActive(isSelect);
            }
        }

        private void SetEntryRewards(PossessionObject[] rewards) {
            var gclm = GlobalContainer.LocalizationManager;
            for(int i = 0, l = entryRewardList.Length; i < l; i++) {
                if(i < rewards.Length) {
                    imgEntryRewardList[i].sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", rewards[i].ID)
                    );
                    if(rewards[i].Num > 1) {
                        txtEntryRewardList[i].text = gclm.GetText(rewards[i].Name + ".Name") + "  x" + rewards[i].Num;
                    } else {
                        txtEntryRewardList[i].text = gclm.GetText(rewards[i].Name + ".Name");
                    }
                    entryRewardList[i].SetActive(true);
                } else {
                    entryRewardList[i].SetActive(false);
                }
            }
        }

        private void SetDiscoveryRewards(PossessionObject[] rewards) {
            var gclm = GlobalContainer.LocalizationManager;
            for(int i = 0, l = discoverRewardList.Length; i < l; i++) {
                if(i < rewards.Length) {
                    imgDiscoverRewardList[i].sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", rewards[i].ID)
                    );
                    if(rewards[i].Num > 1) {
                        txtDiscoverRewardList[i].text = gclm.GetText(rewards[i].Name + ".Name") + "  x" + rewards[i].Num;
                    } else {
                        txtDiscoverRewardList[i].text = gclm.GetText(rewards[i].Name + ".Name");
                    }
                    discoverRewardList[i].SetActive(true);
                } else {
                    discoverRewardList[i].SetActive(false);
                }
            }
        }

        public void ClosePanel() {
            TweenA.Add(gameObject, 0.15f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void OpenPanel(BossDetailPanel.BossBattleData data) {
            SetEntryRewards(data.EntryRewards);
            SetDiscoveryRewards(data.DiscoveryRewards);
            SwitchTab(0);
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.15f, 1).From(0);
        }
    }
}
