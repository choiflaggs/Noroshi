using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using Noroshi.Game;

namespace Noroshi.NoroshiDebug {
    public class DebugCharacter : MonoBehaviour {

        [SerializeField] Image imgChara;
        [SerializeField] Text txtCharaName;
        [SerializeField] Text txtLv;
        [SerializeField] Text txtRare;
        [SerializeField] Text txtPromotion;
        [SerializeField] Text[] txtSkillList;
        [SerializeField] Button btnCharaList;

        public Subject<DebugCharacterStatus> OnOpenDetail = new Subject<DebugCharacterStatus>();

        public class DebugCharacterStatus {
            public uint id;
            public uint playerCharacterID;
            public int lv;
            public int rare;
            public int promotion;
            public List<int> skillLv;
            public int index;
        }

        public DebugCharacterStatus debugStatus = new DebugCharacterStatus();

        public void SetCharaData(uint id, int _index) {
            var gcrm = GlobalContainer.RepositoryManager;

            gcrm.LoadCharacterStatusByPlayerCharacterID(id).Do(status => {
                gcrm.CharacterRepository.Get(status.CharacterID).Do(masterData => {
                    txtCharaName.text = GlobalContainer.LocalizationManager.GetText(masterData.TextKey + ".Name");
                }).Subscribe();
                imgChara.sprite = Resources.Load<Sprite>(
                    string.Format("Character/{0}/thumb_1", status.CharacterID)
                );
                debugStatus.id = status.CharacterID;
                debugStatus.playerCharacterID = id;
                debugStatus.index = _index;
                debugStatus.lv = status.Level;
                debugStatus.rare = status.EvolutionLevel;
                debugStatus.promotion = status.PromotionLevel;
                debugStatus.skillLv = new List<int>();
                txtLv.text = status.Level.ToString();
                txtRare.text = status.EvolutionLevel.ToString();
                txtPromotion.text = status.PromotionLevel.ToString();
                for(int i = 1, l = status.ActionLevels.Length; i < l; i++) {
                    if (txtSkillList[i - 1] == null) { return; }
                    debugStatus.skillLv.Add(status.ActionLevels[i]);
                    txtSkillList[i - 1].text = status.ActionLevels[i].ToString();
                }
            }).Subscribe();
        }

        public void SetLevelValue(int value) {
            debugStatus.lv = value;
            txtLv.text = value.ToString();
        }

        public void SetRarityValue(int value) {
            debugStatus.rare = value;
            txtRare.text = value.ToString();
        }

        public void SetPromotionValue(int value) {
            debugStatus.promotion = value;
            txtPromotion.text = value.ToString();
        }

        public void SetSkillValue(int index, int value) {
            debugStatus.skillLv[index] = value;
            txtSkillList[index].text = value.ToString();
        }

        public void OnClicked() {
            OnOpenDetail.OnNext(debugStatus);
        }
    }
}
