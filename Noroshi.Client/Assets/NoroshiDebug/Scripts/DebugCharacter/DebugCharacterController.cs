using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Noroshi.Game;
using Noroshi.UI;

namespace Noroshi.NoroshiDebug {
    public class DebugCharacterController : MonoBehaviour {
        [SerializeField] GameObject charaContainer;
        [SerializeField] DebugCharacter debugCharacter;
        [SerializeField] DebugCharacterDetail debugCharacterDetail;

        private List<DebugCharacter> debugCharacterList = new List<DebugCharacter>();
        private int openIndex;

        private void Start() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());

            var gcrm = GlobalContainer.RepositoryManager;

            gcrm.PlayerCharacterRepository.GetAll().Do(charaList => {
                SetCharacterList(charaList);
                if (UILoading.Instance != null) {
                    UILoading.Instance.HideLoading();
                }
            }).Subscribe();

            debugCharacterDetail.OnChangeLevel.Subscribe(value => {
                debugCharacterList[openIndex].SetLevelValue(value);
            });
            debugCharacterDetail.OnChangeRarity.Subscribe(value => {
                debugCharacterList[openIndex].SetRarityValue(value);
            });
            debugCharacterDetail.OnChangePromotion.Subscribe(value => {
                debugCharacterList[openIndex].SetPromotionValue(value);
            });
            debugCharacterDetail.OnChangeSkill.Subscribe(d => {
                debugCharacterList[openIndex].SetSkillValue(d["index"], d["value"]);
            });
        }

        private void SetCharacterList(Noroshi.Core.WebApi.Response.PlayerCharacter[] characterList) {
            for(int i = 0, l = characterList.Length; i < l; i++) {
                var dc = Instantiate(debugCharacter);
                dc.transform.SetParent(charaContainer.transform);
                dc.transform.localScale = Vector2.one;
                debugCharacterList.Add(dc);
                dc.SetCharaData(characterList[i].ID, i);
                dc.OnOpenDetail.Subscribe(debugStatus => {
                    Debug.Log("clicked: " + debugStatus.index);
                    openIndex = debugStatus.index;
                    debugCharacterDetail.OpenDetail(debugStatus);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }
        }
    }
}
