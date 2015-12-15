using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class GachaResultItem : MonoBehaviour {
        [SerializeField] Image imgGachaResult;
        [SerializeField] Text txtGachaResult;

        public void Disapper() {
            txtGachaResult.gameObject.SetActive(false);
            TweenA.Add(imgGachaResult.gameObject, 0.001f, 0);
        }

        public void Move(float delay) {
            TweenA.Add(gameObject, 0.25f, 1).EaseOutCubic().Delay(delay).From(0);
            TweenXY.Add(gameObject, 0.25f, transform.localPosition).EaseOutCubic().Delay(delay).From(new Vector2(0, 220));
            TweenR.Add(gameObject, 0.25f, 0).EaseOutCubic().Delay(delay).From(-360).Then(() => {
                txtGachaResult.gameObject.SetActive(true);
            });
            TweenNull.Add(gameObject, delay).Then(() => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });
        }

        public void SetItem(Sprite sprite, string name) {
            imgGachaResult.sprite = sprite;
            txtGachaResult.text = name;
        }
    }
}
