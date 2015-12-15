using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class GearRecipePanel : MonoBehaviour {

        [SerializeField] Text txtGearName;
        [SerializeField] Image imgGear;
        [SerializeField] Text txtCost;
        [SerializeField] RecipeItem[] recipeItemList;
        [SerializeField] BtnCommon btnBack;
        [SerializeField] BtnCommon btnCraft;
        [SerializeField] Image imgRecipeChart;
        [SerializeField] Sprite[] recipeChartSpriteList;

        public Subject<uint> OnMaterialSelect = new Subject<uint>();
        public Subject<uint> OnCreate = new Subject<uint>();
        public Subject<bool> OnBack = new Subject<bool>();

        private uint currentID;
        private List<uint> recipeIDList = new List<uint>();

        private void Start() {
            foreach(var recipeItem in recipeItemList) {
                recipeItem.OnClickedBtn.Subscribe(id => {
                    OnMaterialSelect.OnNext((uint)id);
                });
                recipeItem.OnPlaySE.Subscribe(_ => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            btnBack.OnClickedBtn.Subscribe(_ => {
                OnBack.OnNext(true);
            });
            btnBack.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnCraft.OnClickedBtn.Subscribe(_ => {
                CreateGear();
            });
            btnCraft.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.GET);
            });
        }

        private void CreateGear() {
            var recipe = ItemListManager.Instance.GetGearRecipe(currentID);
            foreach(var r in recipe) {
                ItemListManager.Instance.ChangeItemCount(r["id"], -(int)r["num"]);
            }
            ItemListManager.Instance.ChangeItemCount(currentID, 1);
            SetCreateGearInfo(currentID);
            OnCreate.OnNext(currentID);
        }

        public void SetCreateGearInfo(uint id) {
            var gcrm = GlobalContainer.RepositoryManager;
            var recipes = ItemListManager.Instance.GetGearRecipe(id);
            bool enableCreate = true;

            currentID = id;
            gcrm.GearRepository.Get(id).Do(gear => {
                if(gear != null) {
                    txtGearName.text = GlobalContainer.LocalizationManager.GetText(gear.TextKey + ".Name");
                } else {
                    gcrm.GearPieceRepository.Get(id).Do(gearPiece => {
                        txtGearName.text = GlobalContainer.LocalizationManager.GetText(gearPiece.TextKey + ".Name");
                    }).Subscribe();
                }
            }).Subscribe();
            imgGear.sprite = Resources.Load<Sprite>("Item/" + id);

            recipeIDList = new List<uint>();
            imgRecipeChart.sprite = recipeChartSpriteList[recipes.Count - 1];
            for(var i = 0; i < recipeItemList.Length; i++) {
                if(i < recipes.Count) {
                    recipeItemList[i].SetRecipeInfo(recipes[i]["id"], recipes[i]["num"]);
                    recipeItemList[i].gameObject.SetActive(true);
                    recipeIDList.Add(recipes[i]["id"]);
                } else {
                    recipeItemList[i].gameObject.SetActive(false);
                }
            }
            foreach(var recipe in ItemListManager.Instance.GetGearRecipe(currentID)) {
                if(ItemListManager.Instance.GetItemCount(recipe["id"]) < recipe["num"]) {
                    enableCreate = false;
                }
            }
            if(!enableCreate) {
                btnCraft.SetEnable(false);
            }
        }
    }
}
