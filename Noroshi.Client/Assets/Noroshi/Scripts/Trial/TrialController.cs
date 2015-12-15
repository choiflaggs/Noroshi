using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.WebApi.Response.Trial;

namespace Noroshi.UI {
    public class TrialController : MonoBehaviour {
        public class TrialData {
            public string Name;
            public string Description;
            public bool IsOpen;
            public int RemainNum;
            public int CoolTime;
            public TrialInfo.StageData[] LevelList;
        }

        [SerializeField] TrialPanelController trialPanelController;
        [SerializeField] LevelSelect levelSelect;
        [SerializeField] TrialInfo trialInfo;
        [SerializeField] AlertModal coolTimeAlert;
        [SerializeField] AlertModal noCountAlert;
        
        private int panelNum = 6;
        private List<TrialData> stageDataList = new List<TrialData>();
        private uint[] battleCharacterIdList;
        private int selectedStage;
        private int playerLevel;
        private uint stageID;
        private bool isLoad = false;

        private void Start() {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.ARENA);
            }
            battleCharacterIdList = BattleCharacterSelect.Instance.GetDefaultCharacter(SaveKeys.DefaultTrialBattleCharacter);
            if(battleCharacterIdList.Length < 1) {
                battleCharacterIdList = new uint[]{101};
            }

            trialPanelController.OnMoveEnd.Subscribe(index => {
                selectedStage = index;
                levelSelect.SetPanelInfo(stageDataList[index]);
            });

            levelSelect.OnSelectLevel.Subscribe(id => {
                if(stageDataList[selectedStage].CoolTime > 0) {
                    coolTimeAlert.OnOpen();
                    return;
                }
                if(stageDataList[selectedStage].RemainNum < 1) {
                    noCountAlert.OnOpen();
                    return;
                }
                trialPanelController.SetEnable(false);
                trialInfo.OpenStageInfo(stageDataList[selectedStage].LevelList[id]);
            });

            trialInfo.OnSelectBattle.Subscribe(battleId => {
                var ids = BattleCharacterSelect.Instance.GetPlayerCharacterId(battleCharacterIdList);
                BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultTrialBattleCharacter, ids);
                BattleScene.Bridge.Transition.TransitToCpuBattle(BattleCategory.Trials, (uint)battleId, ids);
            });

            trialInfo.OnEditCharacter.Subscribe(id => {
                stageID = (uint)id;
                BattleCharacterSelect.Instance.OpenPanel(false, battleCharacterIdList);
            });

            trialInfo.OnCloseStageInfo.Subscribe(_ => {
                TweenNull.Add(gameObject, 0.01f).Then(() => {
                    trialPanelController.SetEnable(true);
                });
            });

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleCharacterSelect.Instance.SaveDefaultCharacter(SaveKeys.DefaultTrialBattleCharacter, playerCharacterIds);
                BattleScene.Bridge.Transition.TransitToCpuBattle(BattleCategory.Trials, stageID, playerCharacterIds);
            }).AddTo(this);

            BattleCharacterSelect.Instance.OnClosePanel.Subscribe(list => {
                if(list != null) {
                    battleCharacterIdList = list;
                    trialInfo.SetBattleCharacterIcon(battleCharacterIdList);
                }
                trialInfo.gameObject.SetActive(true);
            }).AddTo(this);

            LoadTrialInfo();
            BattleCharacterSelect.Instance.ReloadCharacterList();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad || !BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            trialInfo.CloseStageInfo();
            trialInfo.SetBattleCharacterIcon(battleCharacterIdList);
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

         private void LoadTrialInfo() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerStatusRepository.Get().Do(data => {
                playerLevel = data.Level;
            }).Subscribe();
            Noroshi.Trial.WebApiRequester.List().Do(response => {
                for(int i = 0; i < panelNum; i++) {
                    if(i < response.Trials.Length) {
                        stageDataList.Add(ArrangeData(response.Trials[i]));
                    } else {
                        stageDataList.Add(null);
                    }
                    trialPanelController.SetPanel(i, stageDataList[i]);
                }
                isLoad = true;
            }).Subscribe();
        }

        private TrialData ArrangeData(Core.WebApi.Response.Trial.Trial data) {
            var trialData = new TrialData();
            var levelDataList = new List<TrialInfo.StageData>();
            var remainingTime = data.ReopenedAt.HasValue ? data.ReopenedAt.Value - GlobalContainer.TimeHandler.UnixTime : 0;
            TimeSpan ts = TimeSpan.FromSeconds(remainingTime);
            int coolTime = ts.Days * 86400 + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
            if(coolTime < 0) {coolTime = 0;}
            trialData.Name = data.TextKey;
            trialData.Description = data.TextKey;
            trialData.IsOpen = data.IsOpen;
            trialData.RemainNum = (int)(Core.Game.Trial.Constant.MAX_BATTLE_NUM - data.BattleNum);
            trialData.CoolTime = coolTime;

            for(int i = 0, l = data.Stages.Length; i < l; i++) {
                var levelData = new TrialInfo.StageData();
                var trialLevel = data.Stages[i];
                levelData.Name = data.TextKey + " Lv" + trialLevel.Rank;
                levelData.Level = trialLevel.Rank;
                levelData.IsOpen = trialLevel.IsOpen;
                levelData.Stamina = 0;
                levelData.EnemyList = new int[] {0, 1, 2, 3, 4};
                levelData.ItemList = new int[] {};
                levelData.StageID = (int)trialLevel.ID;

                levelDataList.Add(levelData);
            }
            trialData.LevelList = levelDataList.ToArray();

            return trialData;
        }
    }
}
