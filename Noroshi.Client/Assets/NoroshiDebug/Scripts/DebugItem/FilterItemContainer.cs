using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.UI;

namespace Noroshi.NoroshiDebug {
    public class FilterItemContainer : MonoBehaviour {
        [SerializeField] BtnCommon overlay;
        [SerializeField] GameObject filterPanel;
        [SerializeField] BtnCommon btnDecide;

        public Subject<Dictionary<string, int>> OnDecideFilter = new Subject<Dictionary<string, int>>();

        private int selectedType;
        private int selectedRarity;

        private void Start() {
            btnDecide.OnClickedBtn.Subscribe(_ => {
                Close();
            });
            btnDecide.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            overlay.OnClickedBtn.Subscribe(_ => {
                Close();
            });
        }

        private void Close() {
            OnDecideFilter.OnNext(new Dictionary<string, int>{
                {"type", selectedType}, {"rarity", selectedRarity}
            });
            TweenA.Add(overlay.gameObject, 0.25f, 0);
            TweenA.Add(filterPanel, 0.25f, 0).EaseOutCubic().Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void Open() {
            gameObject.SetActive(true);
            TweenA.Add(overlay.gameObject, 0.1f, 0.7f).From(0).EaseOutCubic();
            TweenA.Add(filterPanel, 0.2f, 1).From(0).EaseOutCubic();
        }

        public void SelectType(int index) {
            selectedType = index;
        }

        public void SelectRarity(int index) {
            selectedRarity = index;
        }
    }
}
