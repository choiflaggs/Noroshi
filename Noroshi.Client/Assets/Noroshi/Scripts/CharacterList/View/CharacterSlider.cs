using UnityEngine;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class CharacterSlider : MonoBehaviour {
        public Subject<Vector2> OnTapCharacter = new Subject<Vector2>();
        public Subject<float> OnSlideStart = new Subject<float>();
        public Subject<float> OnSlideEnd = new Subject<float>();
        public Subject<float> OnDragCharacter = new Subject<float>();

        private bool isDrag = false;
        private float startPositionX = 0;
        private float disX = 0;
        private float screenRatio;

        private void Start() {
            screenRatio = Constant.SCREEN_BASE_WIDTH / (float)Screen.width;
        }

        private void Tap(TouchData data) {
            if(data.length > 1 && data.index > 0) {return;}
            var screenPosition = new Vector2(
                data.screenPosition.x * screenRatio,
                data.screenPosition.y * screenRatio
            );
            OnTapCharacter.OnNext(screenPosition);
        }

        private void Touch(TouchData data) {
            if(data.length > 1 && data.index > 0) {return;}
            isDrag = true;
            disX = 0;
            startPositionX = data.screenPosition.x;
            OnSlideStart.OnNext(disX);
        }

        private void Drag(TouchData data) {
            if(!isDrag) {return;}
            disX = data.screenPosition.x - startPositionX;
            if(disX > 10 || disX < -10) {
                OnDragCharacter.OnNext(disX);
            }
        }

        private void Release(TouchData data) {
            if(!isDrag) {return;}
            isDrag = false;
            OnSlideEnd.OnNext(disX);
        }

        private void Leave(TouchData data) {
            var posX = data.screenPosition.x * screenRatio;
            var posY = data.screenPosition.y * screenRatio;
            if(!isDrag ||
               (posX < 1086 && posX > 50 && posY < 565 && posY > 25)) {
                return;
            }
            isDrag = false;
            OnSlideEnd.OnNext(disX);
        }
    }
}
