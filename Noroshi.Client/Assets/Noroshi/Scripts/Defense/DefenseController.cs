using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.Game.Enums;

namespace Noroshi.UI {
    public class DefenseController : MonoBehaviour {
        [SerializeField] DefensePanel defensePanel;
        [SerializeField] GameObject levelSelect;
        [SerializeField] BtnCommon[] btnLevelList;
        [SerializeField] BtnCommon btnCloseLevelSelect;
        [SerializeField] Text txtRemainNum;

        private uint stageID;

        private void Start() {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.ARENA);
            }
            defensePanel.SetDefensePanel(501);
            defensePanel.OnClickedBtn.Subscribe(id => {
                OpenLevelSelect();
            });

            foreach(var btn in btnLevelList) {
                btn.OnClickedBtn.Subscribe(id => {
                    if(id != 20101) {return;}
                    stageID = (uint)id;
                    BattleCharacterSelect.Instance.OpenPanel(false);
                });
            }

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleScene.Bridge.Transition.TransitToCpuBattle(BattleCategory.DefensiveWar, stageID, playerCharacterIds);
            }).AddTo(this);

            btnCloseLevelSelect.OnClickedBtn.Subscribe(_ => {
                CloseLevelSelect();
            });

            BattleCharacterSelect.Instance.ReloadCharacterList();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            BattleCharacterSelect.Instance.ClosePanel();
            CloseLevelSelect();
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void OpenLevelSelect() {
            levelSelect.SetActive(true);
            TweenA.Add(levelSelect, 0.2f, 1);
        }

        private void CloseLevelSelect() {
            TweenA.Add(levelSelect, 0.2f, 0).Then(() => {
                levelSelect.SetActive(false);}
            );
        }
    }
}
