using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Noroshi.UI;
using NoroshiDebug.Repositories.Server;

namespace Noroshi.NoroshiDebug {
    public class DebugItem : MonoBehaviour {
        [SerializeField] Image itemImg;
        [SerializeField] Text itemName;
        [SerializeField] Text txtHaveNum;
        [SerializeField] BtnCommon btnMinus;
        [SerializeField] BtnCommon btnPlus;
        [SerializeField] BtnCommon btnMinus10;
        [SerializeField] BtnCommon btnPlus10;

        public Subject<bool> OnStartProcess = new Subject<bool>();
        public Subject<bool> OnEndProcess = new Subject<bool>();

        public uint itemType;
        public uint itemRarity;

        private uint itemID; 
        private uint itemNum;

        private void Start() {
            btnMinus.OnClickedBtn.Subscribe(_ => {
                LoseItem(1);
            });
            btnMinus.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            btnMinus10.OnClickedBtn.Subscribe(_ => {
                LoseItem(10);
            });
            btnMinus10.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnPlus.OnClickedBtn.Subscribe(_ => {
                GetItem(1);
            });
            btnPlus.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });
            btnPlus10.OnClickedBtn.Subscribe(_ => {
                GetItem(10);
            });
            btnPlus10.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });
        }

        public void SetItemData(uint id, string name, uint num, uint type, uint rarity) {
            transform.localScale = Vector3.one;
            itemID = id;
            itemImg.sprite = Resources.Load<Sprite>("Item/" + id);
            itemName.text = name;
            itemNum = num;
            txtHaveNum.text = itemNum.ToString();
            itemType = type;
            itemRarity = rarity;
        }

        private void LoseItem(ushort num) {
            var repo = new PlayerItemDebugRepository();
            if(itemNum <= 0) {
                return;
            } else if(itemNum < num) {
                num = (ushort)itemNum;
            }
            OnStartProcess.OnNext(true);
            repo.UseItem(itemID, num).Do(data => {
                itemNum = data.PossessionsCount;
                txtHaveNum.text = itemNum.ToString();
                OnEndProcess.OnNext(true);
            }).Subscribe();
        }

        public void GetItem(ushort num) {
            var repo = new PlayerItemDebugRepository();
            OnStartProcess.OnNext(true);
            repo.AddItem(itemID, num).Do(data => {
                itemNum = data.PossessionsCount;
                txtHaveNum.text = itemNum.ToString();
                OnEndProcess.OnNext(true);
            }).Subscribe();
        }
    }
}
