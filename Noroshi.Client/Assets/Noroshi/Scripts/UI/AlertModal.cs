using UnityEngine;
using System.Collections;

namespace Noroshi.UI {
    public class AlertModal : MonoBehaviour {

        public void OnOpen() {
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.2f, 1).Then(() => {
                TweenA.Add(gameObject, 1.0f, 0).Delay(1.2f).EaseInCubic().Then(() => {
                    gameObject.SetActive(false);
                });
            });
        }

        public void ResetTween() {
            gameObject.PauseTweens();
            TweenA.Add(gameObject, 0.01f, 0);
        }
    }
}
