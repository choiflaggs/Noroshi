using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class BtnLoadLevel : BtnCommon {
        [SerializeField] string sceneName;

        public Subject<string> OnChangeLevel = new Subject<string>();

        private bool isLoading = true;

        private void Start() {
            if(UILoading.Instance != null) {
                UILoading.Instance.SetLoadBtn(this);
            } else {
                isLoading = false;
            }

            OnClickedBtn.Subscribe(id => {
                if(isLoading) {
                    OnChangeLevel.OnNext(sceneName);
                } else {
                    Application.LoadLevel(sceneName);
                }
            });

            OnPlaySE.Subscribe(_ => {
                if(SoundController.Instance != null) {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                }
            });
        }

        public void SetEnable(bool _isEnable, bool isChangeColor = true) {
            base.SetEnable(_isEnable);
            if(!isChangeColor) {return;}
            if(_isEnable) {
                if(transform.GetChild(0) != null) {
                    TweenC.Add(transform.GetChild(0).gameObject, 0.01f, new Color(1, 1, 1));
                }
                TweenC.Add(gameObject, 0.01f, new Color(1, 1, 1));
            } else {
                if(transform.GetChild(0) != null) {
                    TweenC.Add(transform.GetChild(0).gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
                }
                TweenC.Add(gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
            }
        }

        public void SetSceneName(string sname) {
            sceneName = sname;
        }
    }
}
