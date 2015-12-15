using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GetCharacterModal : MonoBehaviour {
        [SerializeField] Text txtName;
        [SerializeField] BtnCommon btnOK;

        private void Start() {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            btnOK.OnClickedBtn.Subscribe(_ => {
                gameObject.SetActive(false);
            });
            btnOK.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        public void OpenModal(uint id) {
            var character = Instantiate(
                Resources.Load("UICharacter/" + id + "/Character")
            ) as GameObject;
            character.transform.SetParent(transform);
            character.transform.localScale = new Vector3(-25, 25, 25);
            character.transform.localPosition = new Vector3(0, -85, 0);
            character.GetComponent<SkeletonAnimation>().state.SetAnimation(0, Constant.ANIM_RUN, true);

            GlobalContainer.RepositoryManager.CharacterRepository.Get(id).Do(data => {
                txtName.text = GlobalContainer.LocalizationManager.GetText(data.TextKey + ".Name");
            }).Subscribe();

            BattleCharacterSelect.Instance.SetNewCharacter(id);
            SoundController.Instance.PlaySE(SoundController.SEKeys.EVOLUTION);
        }
    }
}
