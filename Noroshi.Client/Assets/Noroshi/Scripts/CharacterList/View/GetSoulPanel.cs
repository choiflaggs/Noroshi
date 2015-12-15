using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Noroshi.UI;
using UniRx;

namespace Noroshi.CharacterList {
    public class GetSoulPanel : MonoBehaviour {
        [SerializeField] GearGetPanel soulGetPanel;
        [SerializeField] BtnCommon btnBackground;

        private void Start() {
            btnBackground.OnClickedBtn.Subscribe(StartCoroutine_Auto => {
                UILoading.Instance.RemoveQuery(QueryKeys.IsOpenGetSoulPanel);
                UILoading.Instance.RemoveQuery(QueryKeys.SelectedUnAcquiredCharacter);
                CloseSoulPanel();
            });
        }

        private void CloseSoulPanel() {
            TweenA.Add(soulGetPanel.gameObject, 0.2f, 0).EaseOutCubic();
            TweenA.Add(btnBackground.gameObject, 0.2f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void OpenSoulPanel() {
            gameObject.SetActive(true);
            TweenA.Add(btnBackground.gameObject, 0.1f, 0.6f).From(0);
            TweenA.Add(soulGetPanel.gameObject, 0.2f, 1).From(0).EaseOutCubic();
        }
    }
}
