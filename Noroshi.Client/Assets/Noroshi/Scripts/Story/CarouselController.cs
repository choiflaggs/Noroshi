using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;

public class CarouselController : MonoBehaviour {
    [SerializeField] GameObject wrapper;
    [SerializeField] int listWidth;
    [SerializeField] BtnCommon btnNext;
    [SerializeField] BtnCommon btnPrev;
    public int listNum;

    public Subject<int> OnMoveEnd = new Subject<int>();

    Vector3 startPosition = Vector3.zero;
    int currentIndex = 0;
    bool isEnable = true;
    bool isDrag = false;
    bool isLeave = false;
    float wrapperWidth;
    float leaveInterval = 0.1f;

    void Start() {
        btnPrev.OnClickedBtn.Subscribe(_ => {
            Prev();
        });
        btnNext.OnClickedBtn.Subscribe(_ => {
            Next();
        });
        Init(currentIndex);
    }

    void Update() {
        if(!isDrag) {return;}
        if(wrapper.transform.localPosition.x > 0) {
            wrapper.transform.localPosition = new Vector3(0, 0, 0);
        }
        if(wrapper.transform.localPosition.x < -wrapperWidth) {
            wrapper.transform.localPosition = new Vector3(-wrapperWidth, 0, 0);
        }
        if(isLeave) {
            leaveInterval -= Time.deltaTime;
            if(leaveInterval < 0) {
                MoveCarousel();
                isLeave = false;
            }
        }
    }

    void Touch(TouchData data) {
        if(!isEnable || (data.length > 1 && data.index > 0)) {return;}
        isDrag = true;
        isLeave = false;
        startPosition.x = wrapper.transform.localPosition.x;
        wrapper.PauseTweens();
    }

    void Drag(TouchData data) {
        if(!isDrag) {return;}
        float newX = 0;
        isLeave = false;
        Vector3 screenDis = Camera.main.WorldToScreenPoint(
            new Vector3(data.distanceX, 0, 0)
        );
        float dx = screenDis.x - Screen.width / 2;

        newX = startPosition.x + dx * Constant.SCREEN_BASE_WIDTH / Screen.width;
        if(newX > 0) {newX = 0;}
        if(newX < -wrapperWidth) {newX = -wrapperWidth;}
        wrapper.transform.localPosition = new Vector3(newX, 0, 0);
    }

    void Release(TouchData data) {
        if(!isDrag) {return;}
        isDrag = false;
        if(!isEnable || (data.length > 1 && data.index > 0)) {return;}
        JudgeMove(data.distanceX);
        MoveCarousel();
    }

    void Leave(TouchData data) {
        var ratio = Constant.SCREEN_BASE_WIDTH / (float)Screen.width;
        var posX = data.screenPosition.x * ratio;
        if((isDrag && posX > Constant.SCREEN_BASE_WIDTH - 50) || (isDrag && posX < 50)) {
            isDrag = false;
            JudgeMove(data.distanceX);
            MoveCarousel();
        } else {
            isLeave = true;
            leaveInterval = 0.1f;
        }
    }

    void JudgeMove(float dx) {
        if(!isEnable) {return;}
        if(dx < -0.1f) {
            currentIndex++;
            if(currentIndex > listNum - 1) {currentIndex = listNum - 1;}
        }
        if(dx > 0.1f) {
            currentIndex--;
            if(currentIndex < 0) {currentIndex = 0;}
        }
    }

    void MoveCarousel() {
        if(!isEnable) {return;}
        int newX = currentIndex * -listWidth;
        wrapper.PauseTweens();
        TweenX.Add(wrapper, 0.5f, newX).EaseOutCubic().Then(() => {
            OnMoveEnd.OnNext(currentIndex);
        });
        if(currentIndex == 0) {
            TweenA.Add(btnPrev.gameObject, 0.3f, 0);
        } else {
            TweenA.Add(btnPrev.gameObject, 0.3f, 1);
        }
        if(currentIndex == listNum - 1) {
            TweenA.Add(btnNext.gameObject, 0.3f, 0);
        } else {
            TweenA.Add(btnNext.gameObject, 0.3f, 1);
        }
    }

    public void Init(int index) {
        currentIndex = index;
        wrapperWidth = listWidth * (listNum - 1);
        MoveCarousel();
    }

    public int GetIndex() {
        return currentIndex;
    }

    public void Next() {
        if(!isEnable) {return;}
        currentIndex++;
        if(currentIndex > listNum - 1) {currentIndex = listNum - 1;}
        MoveCarousel();
    }

    public void Prev() {
        if(!isEnable) {return;}
        currentIndex--;
        if(currentIndex < 0) {currentIndex = 0;}
        MoveCarousel();
    }

    public void EnableCarousel() {
        isEnable = true;
    }

    public void DisableCarousel() {
        isEnable = false;
    }
}
