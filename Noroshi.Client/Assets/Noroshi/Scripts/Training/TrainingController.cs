using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.Game.Enums;

namespace Noroshi.UI {
    public class TrainingController : MonoBehaviour {
        [SerializeField] TrainingPanel[] panelList;
        [SerializeField] StageInfo stageInfo;
        [SerializeField] Text txtTrainingLevel;
        [SerializeField] Text txtRemainNum;
        [SerializeField] Text txtMaxNum;

        private uint stageID;

        private void Start() {
            if(SoundController.Instance != null) {
                SoundController.Instance.PlayBGM(SoundController.BGMKeys.HOME);
            }
            foreach(var panel in panelList) {
                panel.OnClickedBtn.Subscribe(id => {
                    Debug.Log(id);
                });
            }

            BattleCharacterSelect.Instance.OnStartBattle.Subscribe(playerCharacterIds => {
                BattleScene.Bridge.Transition.TransitToCpuBattle(BattleCategory.Training, stageID, playerCharacterIds);
            }).AddTo(this);

            BattleCharacterSelect.Instance.ReloadCharacterList();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!BattleCharacterSelect.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }

            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }
    }
}
