using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class StageScrollController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        [SerializeField] GameObject scrollContent;

        public Subject<bool> OnStartDrag = new Subject<bool>();

        private bool isDrag = false;
        private float screenHeight;
        private float contentHeight;
        private float touchPositionY;

        private void Awake() {
            var ratio =  (float)Screen.height / (float)Screen.width;
            screenHeight = Constant.SCREEN_BASE_WIDTH * ratio;
            contentHeight = scrollContent.GetComponent<RectTransform>().sizeDelta.y;
            GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.SCREEN_BASE_WIDTH, screenHeight);
        }

        public void OnPointerDown(PointerEventData ped) {
            touchPositionY = scrollContent.transform.localPosition.y;
        }
        
        public void OnPointerUp(PointerEventData ped) {
            isDrag = false;
        }

        public void OnDrag(PointerEventData ped) {
            if(!isDrag && 
               Mathf.Abs(scrollContent.transform.localPosition.y - touchPositionY) > 10) {
                OnStartDrag.OnNext(true);
                isDrag = true;
            }
        }

        public void SetPosition(float positionY) {
            TweenY.Add(scrollContent, 0.01f, positionY);
        }

        public void TransitionPosition(float positionY) {
            if(positionY > (contentHeight - screenHeight) / 2) {
                positionY = (contentHeight - screenHeight) / 2;
            }
            if(positionY < -(contentHeight - screenHeight) / 2) {
                positionY = -(contentHeight - screenHeight) / 2;
            }
            TweenY.Add(scrollContent, 0.2f, positionY).EaseOutCubic();
        }
    }
}
