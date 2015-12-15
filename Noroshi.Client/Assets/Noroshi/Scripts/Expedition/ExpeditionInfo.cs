using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LitJson;
using UniRx;
using Noroshi.UI;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.WebApi.Response.Expedition;

namespace Noroshi.UI {
    public class ExpeditionInfo : MonoBehaviour {
        [SerializeField] CharacterStatusIcon[] characterList;
        [SerializeField] Text txtEnemyName;
        [SerializeField] Text txtStageIndex;
        [SerializeField] Text txtMaxStageCount;
        [SerializeField] BtnCommon btnFight;

        public Subject<int> OnSelectBattle = new Subject<int>();

        private void Start() {
            btnFight.OnClickedBtn.Subscribe(id => {
                OnSelectBattle.OnNext(id);
            });
            btnFight.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });
        }

        private CharacterStatusIcon.CharacterData SetCharacterStatus(PlayerCharacter characterStatus) {
            var characterData = new CharacterStatusIcon.CharacterData();
            
            characterData.CharacterID = characterStatus.CharacterID;
            characterData.Level = characterStatus.Level;
            characterData.EvolutionLevel = characterStatus.EvolutionLevel;
            characterData.PromotionLevel = characterStatus.PromotionLevel;
            return characterData;
        }

        public void OpenExpeditionInfo(PlayerExpeditionStage data, int stageLength, int index, bool isClear) {
            var dataLength = data.PlayerCharacters.Length;
            btnFight.gameObject.SetActive(!isClear);
            btnFight.id = (int)data.ID;
            txtEnemyName.text = data.PlayerName.ToString();
            txtStageIndex.text = (index + 1).ToString();
            txtMaxStageCount.text = stageLength.ToString();
            for(int i = 0, l = characterList.Length; i < l; i++) {
                if(i < dataLength) {
                    var characterData = SetCharacterStatus(data.PlayerCharacters[i]);
                    characterList[i].SetInfo(characterData);
                    characterList[i].gameObject.SetActive(true);
                } else {
                    characterList[i].gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.1f, 1);
        }
        
        public void CloseExpeditionInfo() {
            TweenA.Add(gameObject, 0.1f, 0).Then(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
