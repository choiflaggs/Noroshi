using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;
using Noroshi.Core.Game.Player;

namespace Noroshi.CharacterList {
    public class SkillPanel : MonoBehaviour {
        [SerializeField] Image imgSkill;
        [SerializeField] Text txtLevel;
        [SerializeField] Text txtMoney;
        [SerializeField] BtnCommon btnSkillUp;
        [SerializeField] GameObject unlockedContainer;
        [SerializeField] GameObject lockTxt;

        public Subject<int> OnPanelClick = new Subject<int>();
        public Subject<int> OnSkillUp = new Subject<int>();

        public int index;


        private void Start() {
            btnSkillUp.OnClickedBtn.Subscribe(_ => {
                OnSkillUp.OnNext(index);
            });
            btnSkillUp.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.STATUS_UP);
            });
        }

        public void SetSkillPanel(uint id, bool isEnable) {
            imgSkill.sprite = Resources.Load<Sprite>(
                string.Format("UICharacter/{0}/SkillIcon/Skill{1}", id, index + 1)
            );
            unlockedContainer.SetActive(isEnable);
            lockTxt.SetActive(!isEnable);
            if(isEnable) {
                imgSkill.color = Color.white;
            } else {
                imgSkill.color = new Color(0.2f, 0.2f, 0.2f);
            }
        }

        public void UpdateLevel(int level, uint gold) {
            txtLevel.text = level.ToString();
            txtMoney.text = gold.ToString();
        }

        public void SetActiveLevelUp(bool canLevelUp) {
            btnSkillUp.SetEnable(canLevelUp);
        }

        public void OnSkillPanelClick() {
            OnPanelClick.OnNext(index);
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
    }
}
