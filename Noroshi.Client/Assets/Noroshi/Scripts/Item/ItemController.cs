using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.UI {
    public class ItemController : MonoBehaviour {
        [SerializeField] ItemDetail itemDetail;
        [SerializeField] ItemList itemListPref;
        [SerializeField] GameObject itemListWrapper;
        [SerializeField] BtnCommon[] btnFilterList;

        private List<ItemList> itemList = new List<ItemList>();
        private bool isLoad = false;


        private void Start() {
            GlobalContainer.SetFactory(() => new Repositories.RepositoryManager());
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.InventoryRepository.Get().Do(playerItemList => {
                SetGearIconList(playerItemList.Gears);
                SetGearPieceIconList(playerItemList.GearPieces);
                SetDrugIconList(playerItemList.Drugs);
                SetGearEnchantMaterialIconList(playerItemList.GearEnchantMaterials);
                SetSoulIconList(playerItemList.Souls);
                SetExchangeCashGiftIconList(playerItemList.ExchangeCashGifts);
                SetRaidTicketIconList(playerItemList.RaidTickets);
                isLoad = true;
            }).Subscribe();

            foreach(var btn in btnFilterList) {
                btn.OnClickedBtn.Subscribe(id => {
                    foreach(var item in itemList) {
                        if(id == 0 || (id == 1 && item.itemType == 1) ||
                           (id == 2 && item.itemType == 2) || (id == 3 && item.itemType == 3) ||
                           (id == 4 && item.itemType == 4)) {
                            item.gameObject.SetActive(true);
                        } else {
                            item.gameObject.SetActive(false);
                        }
                    }
                    SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
                });
            }

            StartCoroutine("OnLoading");
        }

        private IEnumerator OnLoading() {
            while(!isLoad) {
                yield return new WaitForEndOfFrame();
            }
            if(UILoading.Instance != null) {
                UILoading.Instance.HideLoading();
            }
        }

        private void SetGearIconList(PlayerGear[] gearList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var gear in gearList) {
                gcrm.GearRepository.Get(gear.GearID).Do(itemData => {
                    CreateItemIcon(itemData, gear.PossessionsCount, 1);
                }).Subscribe();
            }
        }

        private void SetGearPieceIconList(PlayerGearPiece[] gearPieceList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var gearPiece in gearPieceList) {
                gcrm.GearPieceRepository.Get(gearPiece.GearPieceID).Do(itemData => {
                    CreateItemIcon(itemData, gearPiece.PossessionsCount, 2);
                }).Subscribe();
            }
        }

        private void SetGearEnchantMaterialIconList(PlayerGearEnchantMaterial[] gearEnchantMaterialList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var gearEnchantMaterial in gearEnchantMaterialList) {
                gcrm.GearEnchantMaterialRepository.Get(gearEnchantMaterial.GearEnchantMaterialID).Do(itemData => {
                    CreateItemIcon(itemData, gearEnchantMaterial.PossessionsCount, 3);
                }).Subscribe();
            }
        }

        private void SetDrugIconList(PlayerDrug[] drugList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var drug in drugList) {
                gcrm.DrugRepository.Get(drug.DrugID).Do(itemData => {
                    CreateItemIcon(itemData, drug.PossessionsCount, 3);
                }).Subscribe();
            }
        }

        private void SetSoulIconList(PlayerSoul[] soulList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var soul in soulList) {
                gcrm.SoulRepository.LoadAll().Do(soulDataList => {
                    foreach(var soulData in soulDataList.Where(s => s.ID == soul.SoulID)) {
                        var possessions = soul.PossessionsCount;
                        CreateItemIcon(soulData, possessions, 4);
                    }
                }).Subscribe();
            }
        }

        private void SetExchangeCashGiftIconList(PlayerExchangeCashGift[] exchangeCashGiftList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var exchangeCashGift in exchangeCashGiftList) {
                gcrm.ExchangeCashGiftRepository.Get(exchangeCashGift.ExchangeCashGiftID).Do(itemData => {
                    CreateItemIcon(itemData, exchangeCashGift.PossessionsCount, 5);
                }).Subscribe();
            }
        }

        private void SetRaidTicketIconList(PlayerRaidTicket[] raidTicketList) {
            var gcrm = GlobalContainer.RepositoryManager;
            foreach(var raidTicket in raidTicketList) {
                gcrm.RaidTicketRepository.Get(raidTicket.RaidTicketID).Do(itemData => {
                    CreateItemIcon(itemData, raidTicket.PossessionsCount, 5);
                }).Subscribe();
            }
        }

        private void CreateItemIcon(Core.WebApi.Response.Item data, uint possession, uint type) {
            if(possession < 1) {return;}
            var item = Instantiate(itemListPref);
            item.SetItemInfo(data, possession, type);
            item.transform.SetParent(itemListWrapper.transform);
            item.transform.localScale = Vector3.one;
            itemList.Add(item);
            item.OnClickedBtn.Subscribe(_ => {
                itemDetail.SetItemDetailData(data, possession);
            });
            item.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }
    }
}
