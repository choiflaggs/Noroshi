using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class TrialPanel : MonoBehaviour {
        [SerializeField] SkeletonAnimation skeletonAnimation;
        [SerializeField] GameObject infoPanel;
        [SerializeField] Text txtRemainNum;
        [SerializeField] Text txtRecoverTime;
        [SerializeField] GameObject recoverHeading;
        [SerializeField] GameObject noOpenAlert;

        public Subject<int> OnClickPanel = new Subject<int>();

        private int id;
        private IDisposable timer;
        private bool isOpen = true;

        public void MoveZindex(float value, float c) {
            c = c * 2 - 1.0f;
            if(c > 1) {c = 1;}
            TweenZ.Add(skeletonAnimation.gameObject, 0.01f, value);
            skeletonAnimation.skeleton.SetColor(new Color(c, c, c));
        }

        public void SetInfo(int index, TrialController.TrialData data) {
            id = index;
            if(data == null || !data.IsOpen) {
                isOpen = false;
            } else {
                txtRemainNum.text = data.RemainNum.ToString();
                if(data.CoolTime <= 0) {
                    txtRecoverTime.gameObject.SetActive(false);
                    recoverHeading.SetActive(false);
                } else {
                    timer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(l => {
                        if(data.CoolTime < 1) {
                            timer.Dispose();
                            txtRecoverTime.gameObject.SetActive(false);
                            recoverHeading.SetActive(false);
                            return;
                        }
                        data.CoolTime--;
                        var m = Mathf.Floor(data.CoolTime / 60);
                        var s = data.CoolTime % 60;
                        var minutes = m < 10 ? "0" + m : m.ToString();
                        var seconds = s < 10 ? "0" + s : s.ToString();
                        txtRecoverTime.text = minutes + ":" + seconds;
                    }).AddTo(this);
                }
            }
        }

        public void ShowInfo() {
            if(isOpen) {
                infoPanel.SetActive(true);
            } else {
                noOpenAlert.SetActive(true);
            }
        }

        public void HideInfo() {
            infoPanel.SetActive(false);
            noOpenAlert.SetActive(false);
        }

        public void Tap(TouchData data) {
            if(data.length > 1 && data.index > 0) {return;}
            OnClickPanel.OnNext(id);
        }
    }
}
