using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class LevelSelect : MonoBehaviour {
        [SerializeField] BtnCommon[] btnList;
        [SerializeField] Text txtTitle;
        [SerializeField] Text txtDescription;

        public Subject<int> OnSelectLevel = new Subject<int>();

        private void Start() {
            foreach(var btn in btnList) {
                btn.OnClickedBtn.Subscribe(id => {
                    OnSelectLevel.OnNext(id);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }
        }

        public void SetPanelInfo(TrialController.TrialData data) {
            if(data == null) {
                txtDescription.text = "準備中";
                txtTitle.gameObject.SetActive(false);
                for(int i = 0, iz = btnList.Length; i < iz; i++) {
                    btnList[i].SetEnable(false);
                    TweenC.Add(btnList[i].gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
                    TweenC.Add(btnList[i].transform.GetChild(1).gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
                }
            } else {
                txtTitle.text = data.Name.ToString();
                txtDescription.text = data.Description.ToString();
                txtTitle.gameObject.SetActive(true);
                for(int i = 0, l = btnList.Length; i < l; i++) {
                    if(!data.IsOpen || !data.LevelList[i].IsOpen) {
                        btnList[i].SetEnable(false);
                        TweenC.Add(btnList[i].gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
                        TweenC.Add(btnList[i].transform.GetChild(1).gameObject, 0.01f, new Color(0.5f, 0.5f, 0.5f));
                    } else {
                        btnList[i].SetEnable(true);
                        TweenC.Add(btnList[i].gameObject, 0.01f, new Color(1, 1, 1));
                        TweenC.Add(btnList[i].transform.GetChild(1).gameObject, 0.01f, new Color(1, 1, 1));
                    }
                }
            }

        }
    }
}
