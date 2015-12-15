using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;
using LitJson;

namespace Noroshi.UI {
    public class BtnCaption : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField] Image img;
        [SerializeField] int index;

        public Subject<int> OnTouchBtn = new Subject<int>();
        public Subject<int> OnReleaseBtn = new Subject<int>();

        public void OnPointerDown(PointerEventData ped) {
            OnTouchBtn.OnNext(index);
        }

        public void OnPointerUp(PointerEventData ped) {
            OnReleaseBtn.OnNext(index);
        }

        public void SetImage(Sprite sprite) {
            img.sprite = sprite;
            img.SetNativeSize();
            gameObject.SetActive(true);
        }
    }
}
