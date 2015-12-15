using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Noroshi.UI {
    public class TrainingPanel : BtnCommon {
        [SerializeField] GameObject[] characterList;
        [SerializeField] Text txtStageTitle;
        [SerializeField] Image imgOpenDateFrame;
        [SerializeField] Text txtOpenDate;
        [SerializeField] GameObject spotLight;
        [SerializeField] GameObject iconLock;
        [SerializeField] Sprite spriteOpenFrame;
        [SerializeField] Text txtScore;

        private bool isActive = true;

        public void SetPanel(Noroshi.Core.WebApi.Response.Training.Training trainingData) {

        }
    }
}
