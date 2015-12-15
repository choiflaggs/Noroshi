using UnityEngine;
using System.Collections;

namespace Noroshi.UI {
    public class BtnExpeditionStage : BtnCommon {
        [SerializeField] GameObject unlockImg;
        [SerializeField] GameObject currentImg;
        [SerializeField] GameObject clearedImg;
        
        public void SetStageState(int state) {
            unlockImg.SetActive(false);
            currentImg.SetActive(false);
            clearedImg.SetActive(false);
            switch (state) {
                case 1: unlockImg.SetActive(true); break;
                case 2: currentImg.SetActive(true); break;
                case 3: clearedImg.SetActive(true); break;
                default : break;
            }
        }
        
    }
}
