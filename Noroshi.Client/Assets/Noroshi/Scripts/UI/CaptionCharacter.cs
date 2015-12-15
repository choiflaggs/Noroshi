using UnityEngine;
using UnityEngine .UI;
using System.Collections;

namespace Noroshi.UI {
    public class CaptionCharacter : MonoBehaviour {
        public class CharacterInfo {
            public string Name;
            public uint Lv;
            public uint PromotionLevel;
            public uint ID;
            public bool IsBoss;
            public string Description;
            public int Index;
        }

        [SerializeField] Text txtName;
        [SerializeField] Text txtLv;
        [SerializeField] Image thumbCharacter;
        [SerializeField] Image imgCharacterFrame;
        [SerializeField] Sprite[] imgFrameList;
        [SerializeField] Text txtDescription;

        public void ShowCaption(CharacterInfo characterInfo, Vector3 position) {
            position.x -= 0.3f;
            position.y += 1.4f;
            txtName.text = characterInfo.Name;
            txtLv.text = characterInfo.Lv.ToString();
            thumbCharacter.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_1", characterInfo.ID)
            );
            imgCharacterFrame.sprite = imgFrameList[characterInfo.PromotionLevel- 1];
            txtDescription.text = characterInfo.Description;
            gameObject.transform.position = position;
            gameObject.SetActive(true);
        }

        public void HideCaption() {
            gameObject.SetActive(false);
        }
    }
}
