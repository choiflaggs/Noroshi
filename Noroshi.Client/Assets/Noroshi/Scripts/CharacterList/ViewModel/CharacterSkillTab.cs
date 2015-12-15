using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.UI;
using Noroshi.Game;
using Noroshi.Core.Game.Player;

namespace Noroshi.CharacterList {
    public class CharacterSkillTab : MonoBehaviour {
        [SerializeField] SkillPanel[] skillPanelList;
        [SerializeField] ScrollRect scrollContent;
        [SerializeField] BtnCommon btnRecover;
        [SerializeField] Text txtRecoverTime;
        [SerializeField] Text txtRemainNum;
        [SerializeField] Text txtMaxNum;
        [SerializeField] Text txtSkillDescription;
        [SerializeField] AlertModal notEnoughGoldAlert;
        [SerializeField] ActionLevelPointRecoverModal actionLevelPointRecover;
        [SerializeField] GameObject processing;

        public Subject<int> OnSelectSkill = new Subject<int>();

        private CharacterPanel.CharaData charaData;
        private int remainNum;
        private List<string> skillNameList = new List<string>();
        private List<string> skillDescriptionList = new List<string>();
        private int[] skillupValueList = new int[5] {0, 0, 0, 0, 0};
        private bool isRecoverOpen = false;

        private Noroshi.ActionLevelUpPayment.WebApiRequester actionLevelUpPaymentRequester = new Noroshi.ActionLevelUpPayment.WebApiRequester();
        private Core.WebApi.Response.ActionLevelUpPayment[] actionLevelUpPaymentList;
        private ActionLevelPointHandler _actionLevelPointHandler = new ActionLevelPointHandler();
        private IDisposable actionLevelPointTimer;
        private byte lastActionLevelPoint;
        private uint lastActionLevelPointUpdatedAt;
        private ushort CurrentActionLevelPoint {
            get {
                return _actionLevelPointHandler.CurrentValue(
                    lastActionLevelPoint,
                    lastActionLevelPointUpdatedAt,
                    MaxActionLevelPoint,
                    GlobalContainer.TimeHandler.UnixTime
                );
            }
        }
        private byte MaxActionLevelPoint {
            // temp
            get {
                byte maxValue;
                if(PlayerInfo.Instance.GetPlayerStatus().VipLevel > 4) {
                    maxValue = Noroshi.Core.Game.Player.Constant.MAX_ACTION_LEVEL_POINT;
                } else {
                    maxValue = Noroshi.Core.Game.Player.Constant.MAX_ACTION_LEVEL_POINT / 2;
                }
                return maxValue;
            }
        }

        private void Start() {
            actionLevelUpPaymentRequester.MasterData().Do(list => {
                actionLevelUpPaymentList = list;
            }).Subscribe();

            foreach(var panel in skillPanelList) {
                panel.OnPanelClick.Subscribe(index => {
                    txtSkillDescription.text = skillDescriptionList[index];
                    txtSkillDescription.gameObject.SetActive(true);
                    OnSelectSkill.OnNext(index);
                });
                panel.OnSkillUp.Subscribe(index => {
                    var payment = (int)actionLevelUpPaymentList[charaData.skillLvList[index] - 1].Gold;
                    if(payment > PlayerInfo.Instance.HaveGold) {
                        notEnoughGoldAlert.OnOpen();
                    } else if(remainNum > 0) {
                        remainNum--;
                        PlayerInfo.Instance.ChangeHaveGold(-payment);
                        charaData.skillLvList[index]++;
                        skillupValueList[index]++;
                        skillPanelList[index].UpdateLevel(charaData.skillLvList[index], actionLevelUpPaymentList[charaData.skillLvList[index] - 1].Gold);
                        UpdateRemainNum();
                        if(!CheckSkillLimit(index)) {
                            skillPanelList[index].SetActiveLevelUp(false);
                        }
                    }
                });
            }

            btnRecover.OnClickedBtn.Subscribe(_ => {
                isRecoverOpen = true;
                SendSkillUpValue();
            });

            actionLevelPointRecover.OnRecoverActionLevelPoint.Subscribe(_ => {
                lastActionLevelPoint = PlayerInfo.Instance.GetPlayerStatus().LastActionLevelPoint;
                lastActionLevelPointUpdatedAt = PlayerInfo.Instance.GetPlayerStatus().LastActionLevelPointUpdatedAt;
                remainNum = CurrentActionLevelPoint;
                txtMaxNum.text = MaxActionLevelPoint.ToString();
                UpdateRemainNum();
            });

            OnSkillScroll();
            StartCoroutine("OnLoadingPlayerInfo");
        }

        private IEnumerator OnLoadingPlayerInfo() {
            while (!PlayerInfo.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            Init();
        }

        private void Init() {
            lastActionLevelPoint = PlayerInfo.Instance.GetPlayerStatus().LastActionLevelPoint;
            lastActionLevelPointUpdatedAt = PlayerInfo.Instance.GetPlayerStatus().LastActionLevelPointUpdatedAt;
            remainNum = CurrentActionLevelPoint;
            txtMaxNum.text = MaxActionLevelPoint.ToString();
            UpdateRemainNum();
            actionLevelPointTimer = Observable.Interval(TimeSpan.FromSeconds(1)).Scan(CurrentActionLevelPoint, (prev, _) => {
                if(prev != CurrentActionLevelPoint) {UpdateRemainNum();}
                if(CurrentActionLevelPoint >= MaxActionLevelPoint) {
                    actionLevelPointTimer.Dispose();
                }
                return CurrentActionLevelPoint;
            }).Subscribe().AddTo(this);
        }

        private void OnDisable() {
            SendSkillUpValue();
        }

        private bool CheckSkillLimit(int index) {
            bool enableLvup = false;

            if(PlayerInfo.Instance.GetTutorialStep() < TutorialStep.ClearStoryStage3) {
                enableLvup = false;
            } else {
                enableLvup = charaData.skillLvList[index] < charaData.lv ? true : false;
            }
            return enableLvup;
        }

        private void UpdateRemainNum() {
            if(remainNum < MaxActionLevelPoint) {
                txtRecoverTime.gameObject.SetActive(true);
            } else {
                txtRecoverTime.gameObject.SetActive(false);
            }
            if (remainNum == 0) {
                btnRecover.gameObject.SetActive(true);
            } else {
                btnRecover.gameObject.SetActive(false);
            }
            txtRemainNum.text = remainNum.ToString();
        }

        private void SendSkillUpValue() {
            var gcrm = GlobalContainer.RepositoryManager;
            var sendNum = 0;

            for(int i = 0, l = skillupValueList.Length; i < l; i++) {
                if(skillupValueList[i] > 0) {
                    var index = i + 1;
                    sendNum++;
                    if(isRecoverOpen) {processing.SetActive(true);}
                    gcrm.PlayerCharacterRepository.UpActionLevel(
                        charaData.playerCharacterID, (ushort)skillupValueList[index - 1], (byte)index
                    ).Do(_ => {
                        sendNum--;
                        if(sendNum == 0 && isRecoverOpen) {
                            isRecoverOpen = false;
                            processing.SetActive(false);
                            actionLevelPointRecover.Open(MaxActionLevelPoint);
                        }
                    }).Subscribe();
                }
            }
            if(sendNum == 0 && isRecoverOpen) {
                isRecoverOpen = false;
                actionLevelPointRecover.Open(MaxActionLevelPoint);
            }

            skillupValueList = new int[5]{0, 0, 0, 0, 0};
        }

        public void SetData(CharacterPanel.CharaData data) {
            int n = 1;

            SendSkillUpValue();
            charaData = data;
            if(data.promotionLv > 10)      {n = 5;}
            else if (data.promotionLv > 6) {n = 4;}
            else if (data.promotionLv > 3) {n = 3;}
            else if (data.promotionLv > 1) {n = 2;}

            txtSkillDescription.gameObject.SetActive(false);
            skillNameList = new List<string>();
            skillDescriptionList = new List<string>();
            for(int i = 0, l = data.skillLvList.Count; i < l; i++) {
                bool isEnable = i < n ? true : false;
                skillPanelList[i].SetSkillPanel(charaData.characterID, isEnable);
                skillPanelList[i].UpdateLevel(data.skillLvList[i], actionLevelUpPaymentList[data.skillLvList[i] - 1].Gold);
                skillNameList.Add("skill" + (i + 1));
                skillDescriptionList.Add("description : skill" + (i + 1));
                skillPanelList[i].SetActiveLevelUp(CheckSkillLimit(i));
            }
            scrollContent.verticalNormalizedPosition = 1;
        }

        public void OnSkillScroll() {
            foreach(var panel in skillPanelList) {
                var posX = panel.transform.position.y - 1.2f;
                var pos = panel.transform.localPosition;
                pos.x = 340 - posX * posX * 7;
                panel.transform.localPosition = pos;
            }
        }
    }
}
