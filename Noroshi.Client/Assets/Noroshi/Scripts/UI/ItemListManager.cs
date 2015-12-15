using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.UI {
    public class ItemListManager : MonoBehaviour {

        public static ItemListManager Instance;

        public bool isLoad = false;

        private Dictionary<string, uint> itemNumList;
        private Dictionary<string, Core.WebApi.Response.Gear> gearDetailList;
        private Dictionary<string, List<Dictionary<string, uint>>> recipeList;

        private bool isItemNumLoad = false;
        private bool isRecipeListLoad = false;

        private void Awake() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            if(Instance == null) {Instance = this;}
            SetItemNumList();
            SetGearDetailList();
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isItemNumLoad || !isRecipeListLoad) {
                yield return new WaitForEndOfFrame();
            }
            isLoad = true;
        }

        private void SetItemNumList() {
            var gcrm = GlobalContainer.RepositoryManager;
            itemNumList = new Dictionary<string, uint>();
            gcrm.InventoryRepository.Get().Do(itemList => {
                foreach(var gear in itemList.Gears) {
                    itemNumList["item" + gear.GearID] = gear.PossessionsCount;
                }
                foreach(var gearPiece in itemList.GearPieces) {
                    itemNumList["item" + gearPiece.GearPieceID] = gearPiece.PossessionsCount;
                }
                foreach(var gearEnchantMaterial in itemList.GearEnchantMaterials) {
                    itemNumList["item" + gearEnchantMaterial.GearEnchantMaterialID] = gearEnchantMaterial.PossessionsCount;
                }
                foreach(var drug in itemList.Drugs) {
                    itemNumList["item" + drug.DrugID] = drug.PossessionsCount;
                }
                foreach(var soul in itemList.Souls) {
                    itemNumList["item" + soul.SoulID] = soul.PossessionsCount;
                }
                foreach(var exchangeCashGift in itemList.ExchangeCashGifts) {
                    itemNumList["item" + exchangeCashGift.ExchangeCashGiftID] = exchangeCashGift.PossessionsCount;
                }
                foreach(var raidTicket in itemList.RaidTickets) {
                    itemNumList["item" + raidTicket.RaidTicketID] = raidTicket.PossessionsCount;
                }
                isItemNumLoad = true;
            }).Subscribe();
        }

        private void SetGearDetailList() {
            var gcrm = GlobalContainer.RepositoryManager;
            gearDetailList = new Dictionary<string, Core.WebApi.Response.Gear>();
            recipeList = new Dictionary<string, List<Dictionary<string, uint>>>();
            gcrm.GearRepository.LoadAll().Do(gears => {
                var gCount = 0;
                foreach(var gear in gears) {
                    var g = gear;
                    if(g.ID == 0) {continue;}
                    gearDetailList["gear" + gear.ID] = gear;
                    gcrm.GearRecipeRepository.GetRecipe(g.ID).Do(recipes => {
                        var tempList = new List<Dictionary<string, uint>>();
                        foreach(var recipe in recipes) {
                            tempList.Add(new Dictionary<string, uint>(){
                                {"id", recipe.MaterialItemID}, {"num", recipe.Count}
                            });
                        }
                        gCount++;
                        recipeList["recipe" + g.ID] = tempList;
                        if(gCount == gears.Length) {
                            isRecipeListLoad = true;
                        }
                    }).Subscribe();
                }
            }).Subscribe();
        }

        public uint GetItemCount(uint id) {
            return itemNumList.ContainsKey("item" + id) ? itemNumList["item" + id] : 0;
        }

        public void ChangeItemCount(uint id, int num) {
            if(itemNumList.ContainsKey("item" + id)) {
                int tempNum = (int)itemNumList["item" + id];
                tempNum += num;
                itemNumList["item" + id] = (uint)tempNum;
            } else {
                itemNumList["item" + id] = (uint)num;
            }
        }

        public Core.WebApi.Response.Gear GetGearInfo(uint id) {
            return gearDetailList.ContainsKey("gear" + id) ? gearDetailList["gear" + id] : null;
        }

        public List<Dictionary<string, uint>> GetGearRecipe(uint id) {
            return recipeList.ContainsKey("recipe" + id) ? recipeList["recipe" + id] : null;
        }
    }
}
