using UnityEngine;
using System.Collections.Generic;

public class TouchController2D : MonoBehaviour {
    [SerializeField] int rayDistance = 1000;

    private Camera cam;
    private List<RaycastHit2D> hitList;
    private List<GameObject> selectedList;
    private List<TouchReceiver> touchReceiverList;
    private List<Vector2> worldPositionList;

    void Awake() {
        cam = GetComponent<Camera>();
        Reset();
    }

    void Update () {
        GetTouchState();
    }

    private void GetTouchState() {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) {
            Vector2 touchPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(touchPoint, Vector2.zero, rayDistance);
            hitList = new List<RaycastHit2D>();

            foreach(RaycastHit2D hit in hits) {
                if(hit.collider != null) {
                    hitList.Add(hit);
                }
            }

            if(hitList.Count > 0) {
                bool isSame = true;
                for(int i = 0, l = selectedList.Count; i < l; i++) {
                    if(hitList.Count < selectedList.Count
                        || (hitList[i].collider != null
                            && selectedList[i] != hitList[i].collider.gameObject)
                    ) {
                        isSame = false;
                        break;
                    }
                }
                if(!isSame && selectedList.Count > 0
                   && touchReceiverList.Count > 0
                ) {
                    Reset();
                    foreach(RaycastHit2D hit in hitList) {
                        GameObject obj = hit.collider.gameObject;
                        TouchReceiver receiver = obj.GetComponent<TouchReceiver>();
                        if(receiver != null) {
                            touchReceiverList.Add(receiver);
                            worldPositionList.Add(hit.point);
                        }
                    }
                    for(int i = 0, l = touchReceiverList.Count; i < l; i++) {
                        touchReceiverList[i].ReceiveLeave(worldPositionList[i], Input.mousePosition, i, l);
                    }
                    Reset();
                } else {
                    Reset();
                    foreach(RaycastHit2D hit in hitList) {
                        GameObject obj = hit.collider.gameObject;
                        TouchReceiver receiver = obj.GetComponent<TouchReceiver>();
                        if(receiver != null) {
                            selectedList.Add(obj);
                            touchReceiverList.Add(receiver);
                            worldPositionList.Add(hit.point);
                        }
                    }
                }
            } else {
                if(touchReceiverList.Count > 0) {
                    for(int i = 0, l = touchReceiverList.Count; i < l; i++) {
                        touchReceiverList[i].ReceiveLeave(worldPositionList[i], Input.mousePosition, i, l);
                    }
                }
                Reset();
            }
        }
        if(touchReceiverList.Count < 1
            || touchReceiverList.Count != worldPositionList.Count) {return;}

        if(Input.GetMouseButtonDown(0)) {
            for(int i = 0, l = touchReceiverList.Count; i < l; i++) {
                touchReceiverList[i].ReceiveStart(worldPositionList[i], Input.mousePosition, i, l);
            }
        }
        if(Input.GetMouseButton(0)) {
            for(int i = 0, l = touchReceiverList.Count; i < l; i++) {
                touchReceiverList[i].ReceiveMove(worldPositionList[i], Input.mousePosition, i, l);
            }
        }
        if(Input.GetMouseButtonUp(0)) {
            for(int i = 0, l = touchReceiverList.Count; i < l; i++) {
                touchReceiverList[i].ReceiveEnd(worldPositionList[i], Input.mousePosition, i, l);
            }
            Reset();
        }
    }

    private void Reset() {
        selectedList = new List<GameObject>();
        touchReceiverList = new List<TouchReceiver>();
        worldPositionList = new List<Vector2>();
    }
}
