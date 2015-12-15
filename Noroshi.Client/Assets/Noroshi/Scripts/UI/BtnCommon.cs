using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UniRx;

public class BtnCommon : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] Image imgBtn;
    [SerializeField] Sprite imgOriginal;
    [SerializeField] Sprite imgTap;
    [SerializeField] Sprite imgDisable;
    [SerializeField] Sprite imgUnselected;
    [SerializeField] bool isTransition = true;

    public Subject<int> OnTouchBtn = new Subject<int>();
    public Subject<int> OnReleaseBtn = new Subject<int>();
    public Subject<int> OnClickedBtn = new Subject<int>();
    public Subject<bool> OnPlaySE = new Subject<bool>();

    public int id;
    public bool isEnable = true;

    private float originPositionY = 9999;

    public void SetEnable(bool _isEnable) {
        isEnable = _isEnable;
        if(!isEnable) {
            if(imgDisable != null) {imgBtn.sprite = imgDisable;}
        } else {
            if(imgOriginal != null) {imgBtn.sprite = imgOriginal;}
        }
    }

    public void SetSelect(bool _isSelect) {
        if(!isEnable) {return;}
        if(!_isSelect) {
            if(imgUnselected != null) {imgBtn.sprite = imgUnselected;}
        } else {
            if(imgOriginal != null) {imgBtn.sprite = imgOriginal;}
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) {
        if(originPositionY == 9999) {
            originPositionY = gameObject.transform.localPosition.y;
        }
        if(!isEnable) {return;}
        if(isTransition) {
            if(imgTap != null) {
                imgBtn.sprite = imgTap;
            }
            TweenY.Add(gameObject, 0.01f, originPositionY - 2);
        }
        OnTouchBtn.OnNext(id);
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        TweenY.Add(gameObject, 0.01f, originPositionY);
        if(!isEnable) {return;}
        if(isTransition) {
            if(imgTap != null) {
                imgBtn.sprite = imgOriginal;
            }
        }
        OnReleaseBtn.OnNext(id);
    }

    public virtual void OnPointerClick(PointerEventData ped) {
        if(!isEnable) {return;}
        OnPlaySE.OnNext(true);
        Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => {
            OnClickedBtn.OnNext(id);
        });
    }
}
