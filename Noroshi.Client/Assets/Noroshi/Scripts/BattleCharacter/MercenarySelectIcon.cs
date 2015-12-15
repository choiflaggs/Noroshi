using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class MercenarySelectIcon : CharacterSelectIcon {
        [SerializeField] Text txtName;
        [SerializeField] Text txtGold;

        public void SetMercenaryInfo(string playerName, uint cost) {
            txtName.text = playerName;
            txtGold.text = cost.ToString();
        }
    }
}
