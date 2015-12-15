using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class GearGetPanel : MonoBehaviour {
        [SerializeField] Text txtGearName;
        [SerializeField] BtnCommon btnBack;

        public Subject<bool> OnBack = new Subject<bool>();

        private void Start() {
            btnBack.OnClickedBtn.Subscribe(_ => {
                OnBack.OnNext(true);
            });
            btnBack.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
        }

        public void SetInfo(uint id) {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.GearRepository.Get(id).Do(gear => {
                if(gear != null) {
                    txtGearName.text = GlobalContainer.LocalizationManager.GetText(gear.TextKey + ".Name");
                } else {
                    gcrm.GearPieceRepository.Get(id).Do(gearPiece => {
                        txtGearName.text = GlobalContainer.LocalizationManager.GetText(gearPiece.TextKey + ".Name");
                    }).Subscribe();
                }
            }).Subscribe();
        }
    }
}
