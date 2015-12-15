using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using LitJson;
using Noroshi.Game;

namespace Noroshi.UI {
    public class ArenaBattleDeck : MonoBehaviour {
        [SerializeField] CharacterStatusIcon[] battleCharacterList;
        [SerializeField] BtnCommon btnBattleCharacterEdit;

        private uint[] battleCharacterIdList;

        private void Start() {
            btnBattleCharacterEdit.OnClickedBtn.Subscribe(_ => {
                BattleCharacterSelect.Instance.OpenPanel(true, battleCharacterIdList);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            BattleCharacterSelect.Instance.OnClosePanel.Subscribe(list => {
                if(list != null) {
                    SetBattleCharacterIcon(list);
                    var pcIdList = BattleCharacterSelect.Instance.GetPlayerCharacterId(list);
                    GlobalContainer.RepositoryManager.PlayerArenaRepository.ChangeDeck(
                        pcIdList
                    ).Subscribe();
                }
            }).AddTo(this);
        }

        private CharacterStatusIcon.CharacterData SetCharacterStatus(CharacterStatus characterStatus) {
            var characterData = new CharacterStatusIcon.CharacterData();

            characterData.CharacterID = characterStatus.CharacterID;
            characterData.Level = characterStatus.Level;
            characterData.EvolutionLevel = characterStatus.EvolutionLevel;
            characterData.PromotionLevel = characterStatus.PromotionLevel;
            characterData.IsDeca = characterStatus.TagSet.IsDeca;
            return characterData;
        }

        public void SetBattleCharacterIcon(uint[] idList) {
            var gcrm = GlobalContainer.RepositoryManager;
            var ids = BattleCharacterSelect.Instance.GetPlayerCharacterId(idList);
            battleCharacterIdList = idList;
            for(int i = 0, l = battleCharacterList.Length; i < l; i++) {
                if(i < ids.Length) {
                    int n = i;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(ids[n]).Do(characterStatus => {
                        var characterData = SetCharacterStatus(characterStatus);
                        battleCharacterList[n].SetInfo(characterData);
                    }).Subscribe();
                    battleCharacterList[n].gameObject.SetActive(true);
                } else {
                    battleCharacterList[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
