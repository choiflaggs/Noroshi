using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniLinq;

namespace Noroshi.UI {
    public class AppearRaidBossModal : MonoBehaviour {
        [SerializeField] Canvas canvas;
        [SerializeField] Text txtBossName;
        [SerializeField] Text txtBossLevel;
        [SerializeField] Text txtBossHP;
        [SerializeField] GameObject characterContainer;
        [SerializeField] Image imgRaidBoss;
        [SerializeField] Image[] imgRewardList;
        [SerializeField] BtnLoadLevel btnGoRaidBoss;
        [SerializeField] BtnCommon btnQuit;

        private float charaSize = 20;

        private void Start() {
            canvas.worldCamera = Camera.main;

            btnQuit.OnClickedBtn.Subscribe(_ => {
                CloseModal();
            });
        }

        private void CloseModal() {
            characterContainer.SetActive(false);
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void OpenModal(Noroshi.Core.WebApi.Response.RaidBoss.RaidBoss bossData) {
            var rewards = bossData.DiscoveryRewards;
            var character = Instantiate(
                Resources.Load("UICharacter/" + 501 + "/Character")
            ) as GameObject;
            character.transform.SetParent(characterContainer.transform);
            character.transform.localScale = new Vector2(charaSize, charaSize);
            character.transform.localPosition = Vector3.zero;
            character.GetComponent<MeshRenderer>().sortingOrder = 31;

            txtBossName.text = bossData.TextKey;
            txtBossLevel.text = bossData.Level.ToString();
            txtBossHP.text = bossData.MaxHP.ToString();

            rewards.Concat(bossData.EntryRewards);
            for(int i = 0, l = imgRewardList.Length; i < l; i++) {
                if(i < rewards.Length) {
                    imgRewardList[i].sprite = Resources.Load<Sprite>(
                        string.Format("Item/{0}", rewards[i].ID)
                    );
                    imgRewardList[i].gameObject.SetActive(true);
                } else {
                    imgRewardList[i].gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(true);
        }
    }
}
