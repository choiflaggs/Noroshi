using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;

namespace Noroshi.UI {
    public class TrialPanelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
        public Subject<bool> OnMoveStart = new Subject<bool>();
        public Subject<int> OnMoveEnd = new Subject<int>();

        private int panelNum = 0;
        private int xEye = 0;
        private int yEye = 350;
        private float yPosition = -410;
        private int radius;
        private int cLength = 1000;

        private int crntIndex = 0;
        private float startPositionX = 0;
        private float startAngle = 0;
        private float angle = Mathf.Deg2Rad * (-90);
        private List<TrialPanel> panelList = new List<TrialPanel>();
        private bool isEnable = true;

        private void MovePanel() {
            for (int i = 0; i < panelNum; i++) {
                var tempX = radius * Mathf.Cos(angle + (float)i / (float)panelNum * Mathf.PI * 2);
                var tempZ = radius * Mathf.Sin(angle + (float)i / (float)panelNum * Mathf.PI * 2);
                var zRatio = cLength / (cLength + tempZ) * (-0.015f * panelNum + 0.8f);

                panelList[i].transform.localPosition = new Vector3(
                    xEye + tempX * zRatio, yEye + yPosition * zRatio
                );
                panelList[i].transform.localScale = Vector3.one * zRatio;
                panelList[i].MoveZindex(tempZ, zRatio);
            }
        }

        public void SetPanel(int index, TrialController.TrialData data) {
            var panelObject = Instantiate(
                Resources.Load(string.Format("Trial/{0}/TrialPanel", index + 1))
            ) as GameObject;
            var panel = panelObject.GetComponent<TrialPanel>();
            panelList.Add(panel);
            panel.transform.SetParent(gameObject.transform);
            panel.transform.localScale = Vector3.one;
            panel.SetInfo(index, data);
            panel.HideInfo();
            panel.OnClickPanel.Subscribe(id => {
                if(!isEnable) {return;}
                foreach(var p in panelList) {
                    p.HideInfo();
                }
                crntIndex = id;
                OnPointerUp();
            });
            panelNum = panelList.Count;
            radius = panelNum * 15 + 200;
            panelList[crntIndex].ShowInfo();
            MovePanel();
            OnMoveEnd.OnNext(crntIndex);
        }

        public void OnPointerDown(PointerEventData ped) {
            DOTween.KillAll();
            foreach(var panel in panelList) {
                panel.HideInfo();
            }
            startPositionX = ped.position.x;
            startAngle = angle;
            OnMoveStart.OnNext(true);
        }

        public void OnPointerUp(PointerEventData ped = null) {
            var endPositionX = ped != null ? ped.position.x : startPositionX;
            var diffX = startPositionX - endPositionX;
            var targetRad = 0.0f;
            var targetDeg = 0.0f;
            var deg = Mathf.Rad2Deg * angle;

            if(diffX < -50) {
                crntIndex--;
                if(crntIndex < 0) {crntIndex = panelNum - 1;}
            }
            if(diffX > 50) {
                crntIndex++;
                if(crntIndex > panelNum - 1) {crntIndex = 0;}
            }
            if(deg > 360) {
                angle = Mathf.Deg2Rad * (deg - 360);
                deg = Mathf.Rad2Deg * angle;
            }
            if(deg < -360) {
                angle = Mathf.Deg2Rad * (deg + 360);
                deg = Mathf.Rad2Deg * angle;
            }
            targetDeg = -crntIndex * 360 / panelNum + 270;
            if(deg - targetDeg > 180) {
                targetDeg += 360;
                if(deg - targetDeg > 180) {targetDeg += 360;}
            }
            if(deg - targetDeg < -180) {
                targetDeg -= 360;
                if(deg - targetDeg < -180) {targetDeg -= 360;}
            }
            targetRad = Mathf.Deg2Rad * targetDeg;
            DOTween.KillAll();
            DOTween.To(() => angle, (a) => angle = a, targetRad, 0.5f)
                .OnUpdate(MovePanel)
                .OnComplete(() => {
                    panelList[crntIndex].ShowInfo();
                    OnMoveEnd.OnNext(crntIndex);
                }
            );
        }

        public void OnDrag(PointerEventData ped) {
            var newX = startPositionX + ped.position.x;
            angle = startAngle - (startPositionX - ped.position.x) / 200;
            MovePanel();
        }

        public void SetEnable(bool _isEnable) {
            isEnable = _isEnable;
        }
    }
}
