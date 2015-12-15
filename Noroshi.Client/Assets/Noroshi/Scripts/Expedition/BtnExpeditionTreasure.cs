using UnityEngine;
using System.Collections;

namespace Noroshi.UI {
    public class BtnExpeditionTreasure : BtnCommon {
        [SerializeField] GameObject treasureImg;
        [SerializeField] GameObject gainedImg;

        public void SetTreasureState(bool isGain) {
            if(isGain) {
                treasureImg.SetActive(!isGain);
                gainedImg.SetActive(isGain);
            }
        }
    }
}
