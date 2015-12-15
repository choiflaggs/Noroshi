using UnityEngine;
using UnityEngine .UI;
using System.Collections;

namespace Noroshi.UI {
    public class CaptionItem : MonoBehaviour {
        public class ItemInfo {
            public string Name;
            public uint HaveNum;
            public uint NeedLevel = 1;
            public uint Price;
            public uint ID;
            public string Description;
            public int Index;
        }

        [SerializeField] Text txtName;
        [SerializeField] Text txtHaveNum;
        [SerializeField] Text txtNeedLv;
        [SerializeField] Image thumbItem;
        [SerializeField] Text txtDescription;
        
        public void ShowCaption(ItemInfo itemInfo, Vector3 position) {
            position.x -= 0.1f;
            position.y += 1.4f;
            txtName.text = itemInfo.Name;
            txtHaveNum.text = itemInfo.HaveNum.ToString();
            txtNeedLv.text = itemInfo.NeedLevel.ToString();
            thumbItem.sprite = Resources.Load<Sprite>("Item/" + itemInfo.ID);
            txtDescription.text = itemInfo.Description;
            gameObject.transform.position = position;
            gameObject.SetActive(true);
        }
        
        public void HideCaption() {
            gameObject.SetActive(false);
        }
    }
}
