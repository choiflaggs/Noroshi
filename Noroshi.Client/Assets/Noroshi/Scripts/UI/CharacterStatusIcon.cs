using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class CharacterStatusIcon : MonoBehaviour {
        public class CharacterData {
            public uint CharacterID;
            public int Level;
            public int EvolutionLevel;
            public int PromotionLevel;
            public bool IsDeca;
        }

        [SerializeField] Text txtLv;
        [SerializeField] GameObject iconBig;
        [SerializeField] Image imgCharacter;
        [SerializeField] Image imgCharacterFrame;
        [SerializeField] Sprite[] imgFrameList;
        [SerializeField] GameObject[] evolutionStar;

        public void SetInfo(CharacterData status) {
            var skinLv = status.IsDeca ? status.EvolutionLevel : 
                status.EvolutionLevel < 3 ? 1 : status.EvolutionLevel < 5 ? 2 : 3;
            imgCharacter.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_{1}", status.CharacterID, skinLv)
            );
            txtLv.text = string.Format("Lv:{0}", status.Level);
            
            imgCharacterFrame.sprite = imgFrameList[status.PromotionLevel - 1];
            for(int i = 0; i < status.EvolutionLevel; i++) {
                evolutionStar[i].SetActive(true);
            }
        }

    }
}
