using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.UI;

namespace Noroshi.NoroshiDebug {
    public class DebugItemController : MonoBehaviour {
        [SerializeField] GameObject itemContainer;
        [SerializeField] DebugItem debugItem;
        [SerializeField] FilterItemContainer filterContainer;
        [SerializeField] BtnCommon btnFilter;
        [SerializeField] BtnCommon btnGetAll;
        [SerializeField] GameObject processing;

        private List<DebugItem> itemList = new List<DebugItem>();
        private bool isLoad = false;

        private void Awake() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
        }

        private void Start() {
            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!ItemListManager.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            LoadItemList();
            while(!isLoad) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
            foreach(var item in itemList) {
                item.OnStartProcess.Subscribe(_ => {
                    processing.SetActive(true);
                });
                item.OnEndProcess.Subscribe(_ => {
                    processing.SetActive(false);
                });
            }
        }

        private void LoadItemList() {
            var gcrm = GlobalContainer.RepositoryManager;

            gcrm.GearRepository.LoadAll().Do(gears => {
                foreach(var gear in gears) {
                    var di = Instantiate(debugItem);
                    var num = ItemListManager.Instance.GetItemCount(gear.ID);
                    di.transform.SetParent(itemContainer.transform);
                    di.SetItemData(gear.ID, GlobalContainer.LocalizationManager.GetText(gear.TextKey + ".Name"), num, 1, gear.Rarity);
                    itemList.Add(di);
                }
                gcrm.GearPieceRepository.LoadAll().Do(gearPieces => {
                    foreach(var gearPiece in gearPieces) {
                        var di = Instantiate(debugItem);
                        var num = ItemListManager.Instance.GetItemCount(gearPiece.ID);
                        di.transform.SetParent(itemContainer.transform);
                        di.SetItemData(gearPiece.ID, GlobalContainer.LocalizationManager.GetText(gearPiece.TextKey + ".Name"), num, 2, gearPiece.Rarity);
                        itemList.Add(di);
                    }
                    gcrm.DrugRepository.LoadAll().Do(drugs => {
                        foreach(var drug in drugs) {
                            var di = Instantiate(debugItem);
                            var num = ItemListManager.Instance.GetItemCount(drug.ID);
                            di.transform.SetParent(itemContainer.transform);
                            di.SetItemData(drug.ID, GlobalContainer.LocalizationManager.GetText(drug.TextKey + ".Name"), num, 3, drug.Rarity);
                            itemList.Add(di);
                        }
                        gcrm.GearEnchantMaterialRepository.LoadAll().Do(gearEnchantMaterials => {
                            foreach(var gearEnchantMaterial in gearEnchantMaterials) {
                                var di = Instantiate(debugItem);
                                var num = ItemListManager.Instance.GetItemCount(gearEnchantMaterial.ID);
                                di.transform.SetParent(itemContainer.transform);
                                di.SetItemData(gearEnchantMaterial.ID, GlobalContainer.LocalizationManager.GetText(gearEnchantMaterial.TextKey + ".Name"), num, 3, gearEnchantMaterial.Rarity);
                                itemList.Add(di);
                            }
                            gcrm.SoulRepository.LoadAll().Do(souls => {
                                foreach(var soul in souls) {
                                    var di = Instantiate(debugItem);
                                    var num = ItemListManager.Instance.GetItemCount(soul.ID);
                                    di.transform.SetParent(itemContainer.transform);
                                    di.SetItemData(soul.ID, GlobalContainer.LocalizationManager.GetText(soul.TextKey + ".Name"), num, 4, soul.Rarity);
                                    itemList.Add(di);
                                }
                                gcrm.ExchangeCashGiftRepository.LoadAll().Do(exchangeCashGifts => {
                                    foreach(var exchangeCashGift in exchangeCashGifts) {
                                        var di = Instantiate(debugItem);
                                        var num = ItemListManager.Instance.GetItemCount(exchangeCashGift.ID);
                                        di.transform.SetParent(itemContainer.transform);
                                        di.SetItemData(exchangeCashGift.ID, GlobalContainer.LocalizationManager.GetText(exchangeCashGift.TextKey + ".Name"), num, 5, exchangeCashGift.Rarity);
                                        itemList.Add(di);
                                    }
                                    gcrm.RaidTicketRepository.LoadAll().Do(tickets => {
                                        foreach(var ticket in tickets) {
                                            var di = Instantiate(debugItem);
                                            var num = ItemListManager.Instance.GetItemCount(ticket.ID);
                                            di.transform.SetParent(itemContainer.transform);
                                            di.SetItemData(ticket.ID, GlobalContainer.LocalizationManager.GetText(ticket.TextKey + ".Name"), num, 5, ticket.Rarity);
                                            itemList.Add(di);
                                        }
                                        isLoad = true;
                                    }).Subscribe();
                                }).Subscribe();
                            }).Subscribe();
                        }).Subscribe();
                    }).Subscribe();
                }).Subscribe();
            }).Subscribe();

            btnFilter.OnClickedBtn.Subscribe(_ => {
                filterContainer.Open();
            });
            btnFilter.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            filterContainer.OnDecideFilter.Subscribe(d => {
                foreach(var item in itemList) {
                    item.gameObject.SetActive(true);
                    if(d["type"] == 5) {
                        if(item.itemType < 5) {
                            item.gameObject.SetActive(false);
                        }
                    } else if(d["type"] != 0 && d["type"] != item.itemType) {
                        item.gameObject.SetActive(false);
                    }
                    if(d["rarity"] != 0 && d["rarity"] != item.itemRarity) {
                        item.gameObject.SetActive(false);
                    }
                }
            });

            btnGetAll.OnClickedBtn.Subscribe(_ => {
                foreach(var item in itemList) {
                    item.GetItem(10);
                }
            });
            btnGetAll.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });
        }
    }
}
