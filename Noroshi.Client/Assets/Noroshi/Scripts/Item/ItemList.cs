using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.UI {
    public class ItemList : BtnCommon {
        [SerializeField] Image imgItem;
        [SerializeField] Text txtHaveNum;

        public uint itemType;

        public void SetItemInfo(Item data, uint possession, uint type) {
            id = (int)data.ID;
            itemType = type;
            txtHaveNum.text = possession.ToString();
            imgItem.sprite = Resources.Load<Sprite>("Item/" + data.ID);
        }
    }
}
