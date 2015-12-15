using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class RecipeItem : BtnCommon {

        [SerializeField] Image imgRecipe;
        [SerializeField] Text txtHaveNum;
        [SerializeField] Text txtNeedNum;

        public void SetRecipeInfo(uint id, uint needNum) {
            this.id = (int)id;
            if(needNum > ItemListManager.Instance.GetItemCount(id)) {
                txtHaveNum.color = Constant.TEXT_COLOR_NEGATIVE;
            } else {
                txtHaveNum.color = Constant.TEXT_COLOR_NORMAL_DARK;
            }
            imgRecipe.sprite = Resources.Load<Sprite>("Item/" + id);
            txtHaveNum.text = ItemListManager.Instance.GetItemCount(id).ToString();
            txtNeedNum.text = needNum.ToString();
        }

    }
}
