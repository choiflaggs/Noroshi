using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildConfirmPanel : MonoBehaviour {
        [SerializeField] Text txtPlayerName;
        [SerializeField] BtnCommon btnCancel;
        [SerializeField] BtnCommon btnOK;

        public Subject<bool> OnDecide = new Subject<bool>();

        private void Start() {
            btnCancel.OnClickedBtn.Subscribe(_ => {
                ClosePanel();
            });

            btnOK.OnClickedBtn.Subscribe(_ => {
                OnDecide.OnNext(true);
                ClosePanel();
            });
        }

        private void ClosePanel() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void OpenPanel(string name) {
            txtPlayerName.text = name;

            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1).From(0);
        }
    }
}
