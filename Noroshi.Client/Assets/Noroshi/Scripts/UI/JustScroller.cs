using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class JustScroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        public Subject<int> OnScrollEnd = new Subject<int>();

        private ScrollRect scrollRect;
        private RectTransform content;
        private float panelHeight;
        private int panelNum;
        private List<float> dyList;
        private int currentIndex = 0;
        private bool isDrag = false;

        private void Update() {
            if(!isDrag) {return;}
            currentIndex = Mathf.RoundToInt(
                (content.localPosition.y - panelHeight / 2) / panelHeight
            );
            if(currentIndex < 0) {currentIndex = 0;}
            if(currentIndex > panelNum - 1) {currentIndex = panelNum - 1;}
            dyList.Add(content.localPosition.y);
            if(dyList.Count > 5) {
                dyList.RemoveAt(0);
            }
        }

        private void CheckMove() {
            var targetIndex = 0;
            var vy = 0.0f;
            var n = dyList.Count - 1;

            scrollRect.enabled = false;
            while(n > 0) {
                vy += dyList[n] - dyList[n - 1];
                n--;
            }
            vy = vy / (dyList.Count - 1);
            if(Mathf.Abs(vy) < 5 || float.IsNaN(vy)) {
                targetIndex = currentIndex;
            } else {
                targetIndex = Mathf.RoundToInt(vy / 15) + currentIndex;
            }
            if(targetIndex < 0) {targetIndex = 0;}
            if(targetIndex > panelNum - 1) {targetIndex = panelNum - 1;}
            MoveContent(targetIndex);
        }
        
        private void MoveContent(int index) {
            TweenY.Add(content.gameObject, 0.5f, panelHeight * index + panelHeight / 2)
                .EaseOutCubic();
            currentIndex = index;
            OnScrollEnd.OnNext(currentIndex);
        }

        public void Init(int num, int index = 0) {
            scrollRect = GetComponent<ScrollRect>();
            content = scrollRect.content;
            panelHeight = scrollRect.viewport.GetComponent<RectTransform>().sizeDelta.y;
            panelNum = num;
            MoveContent(index);
        }
        
        public void OnPointerDown(PointerEventData ped) {
            isDrag = true;
            scrollRect.enabled = true;
            dyList = new List<float>();
            content.gameObject.PauseTweens();
        }

        public void OnPointerUp(PointerEventData ped) {
            isDrag = false;
            CheckMove();
        }
    }
}
