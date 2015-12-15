using UnityEngine;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildPlayerInfoPanel : MonoBehaviour {
        [SerializeField] BtnCommon btnClose;

        private void Start() {
            btnClose.OnClickedBtn.Subscribe(_ => {
                Close();
            });
        }

        public void Open() {
            gameObject.SetActive(true);
        }

        public void Close() {
            gameObject.SetActive(false);
        }
    }
}
