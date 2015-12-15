using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class ExpUpPanel : MonoBehaviour {
        [SerializeField] BtnCommon btnBackground;
        [SerializeField] GameObject panel;
        [SerializeField] ExpUpItem[] expUpItemList;

        public Subject<int> OnRaiseExp = new Subject<int>();
        public Subject<Dictionary<string, int>> OnSendExp = new Subject<Dictionary<string, int>>();

        private float panelPositionY = 9999;

        private void OnDisable() {
            SendExpUpData();
        }

        private void SendExpUpData() {
            for(int i = 0, l = expUpItemList.Length; i < l; i++) {
                if(expUpItemList[i].GetUseNum() > 0) {
                    OnSendExp.OnNext(new Dictionary<string, int>{
                        {"itemId", expUpItemList[i].GetItemID()},
                        {"useNum", expUpItemList[i].GetUseNum()}
                    });
                    expUpItemList[i].ResetUseNum();
                }
            }
        }

        public void Init() {
            var gcrm = GlobalContainer.RepositoryManager;
            btnBackground.OnClickedBtn.Subscribe(_ => {
                CloseExpPanel();
            });
            gcrm.DrugRepository.LoadAll().Do(drugList => {
                for(int i = 0, l = drugList.Length; i < l; i++) {
                    var haveNum = ItemListManager.Instance.GetItemCount(drugList[i].ID);
                    var expUpValue = (int)drugList[i].CharacterExp;
                    expUpItemList[i].SetExpUpPanel((int)drugList[i].ID, haveNum);
                    expUpItemList[i].OnPanelClick.Subscribe(index => {
                        OnRaiseExp.OnNext(expUpValue);
                    });
                }
            }).Subscribe();
        }

        public void OpenExpUpPanel(uint id) {
            if(panelPositionY == 9999) {
                panelPositionY = panel.transform.localPosition.y;
            }
            gameObject.SetActive(true);
            panel.SetActive(true);
            TweenA.Add(btnBackground.gameObject, 0.1f, 0.2f).From(0).EaseOutCubic();
            TweenA.Add(panel, 0.2f, 1).From(0).EaseOutCubic();
            TweenY.Add(panel, 0.2f, panelPositionY).From(panelPositionY - 40).EaseOutCubic();
        }

        public void CloseExpPanel() {
            TweenA.Add(btnBackground.gameObject, 0.25f, 0);
            TweenY.Add(panel, 0.25f, panelPositionY - 40).EaseOutCubic();
            TweenA.Add(panel, 0.25f, 0).EaseOutCubic().Then(() => {
                panel.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        public void SetEnableExpUpPanel(bool isEnable) {
            foreach(var panel in expUpItemList) {
                panel.SetEnable(isEnable);
            }
        }
    }
}
