using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class MenuController : MonoBehaviour {
        [SerializeField] BtnCommon btnMenu;
        [SerializeField] BtnCommon btnOverlay;
        [SerializeField] GameObject menuListContainer;
        [SerializeField] GameObject menuListFrame;
        [SerializeField] GameObject[] menuList;
        [SerializeField] GameObject iconMenuNotification;
        [SerializeField] GameObject iconCharacterNotification;

        private bool isOpen = false;
        private bool isActive = true;
        private int menuNum;
        private float duration = 0.2f;
        private float delay = 0.03f;

        private void Start() {
            menuNum = menuList.Length;
            btnMenu.OnClickedBtn.Subscribe(_ => {
                if(!isActive) {return;}
                isActive = false;
                if(isOpen) {
                    CloseMenu();
                } else {
                    OpenMenu();
                }
            }).AddTo(this);

            btnOverlay.OnClickedBtn.Subscribe(_ => {
                if(!isActive) {return;}
                isActive = false;
                CloseMenu();
            }).AddTo(this);

            btnMenu.OnPlaySE.Subscribe(_ => {
                if(isOpen) {
                    TweenNull.Add(gameObject, 0.2f).Then(() => {
                        SoundController.Instance.PlaySE(SoundController.SEKeys.MENU);
                    });
                } else {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.MENU);
                }
            });

            btnOverlay.OnPlaySE.Subscribe(_ => {
                TweenNull.Add(gameObject, 0.2f).Then(() => {
                    SoundController.Instance.PlaySE(SoundController.SEKeys.MENU);
                });
            });

            if(Application.loadedLevelName != Constant.SCENE_CHARACTER_LIST) {
                StartCoroutine("OnLoadingItemList");
            }

            CloseMenu();
        }

        private IEnumerator OnLoadingItemList() {
            while(!ItemListManager.Instance.isLoad) {
                yield return new WaitForEndOfFrame();
            }
            CheckCharacterNotification();
        }

        private void OpenMenu() {
            isOpen = true;

            btnOverlay.gameObject.SetActive(true);
            menuListContainer.SetActive(true);
            TweenA.Add(btnOverlay.gameObject, 0.1f, 0.5f);
            TweenSY.Add(menuListFrame, 0.15f, 1).EaseOutCubic();
            for(var i = 0; i < menuNum; i++) {
                menuList[i].SetActive(true);
                if(i == menuNum - 1) {
                    TweenY.Add(menuList[i], duration, i * -95 - 100)
                        .Delay(i * -delay + delay * (menuNum + 4))
                        .EaseOutBackWith(1.1f)
                        .Then(() => {isActive = true;});
                } else {
                    TweenY.Add(menuList[i], duration, i * -95 - 100)
                        .Delay(i * -delay + delay * (menuNum + 4))
                        .EaseOutBackWith(1.1f);
                }
            }
        }

        private void CloseMenu() {
            isOpen = false;
            TweenSY.Add(menuListFrame, 0.12f, 0).Delay(delay * (menuNum + 7)).EaseInCubic().Then(() => {
                TweenA.Add(btnOverlay.gameObject, 0.1f, 0).Then(() => {
                    menuListContainer.SetActive(false);
                    btnOverlay.gameObject.SetActive(false);
                    isActive = true;
                });
            });
            for(var i = 0; i < menuNum; i++) {
                var index = i;
                TweenY.Add(menuList[index], duration, 50)
                    .Delay(i * delay)
                    .EaseInBackWith(1.2f)
                    .Then(() => {
                        menuList[index].SetActive(false);
                    });
            }
            if(!btnOverlay.gameObject.activeSelf) {isActive = true;}
        }

        private void CheckCharacterNotification() {
            var gcrm = GlobalContainer.RepositoryManager;
            gcrm.PlayerCharacterRepository.GetAll().Do(playerCharacterList => {
                foreach(var chara in playerCharacterList) {
                    var c = chara;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(c.ID).Do(character => {
                        gcrm.CharacterRepository.Get(c.CharacterID).Do(masterData => {
                            for(int i = 0, l = character.GearIDs.Length; i < l; i++) {
                                if(character.GearIDs[i] < 1) {
                                    if(c.Level >= ItemListManager.Instance.GetGearInfo(masterData.GearIDs[c.PromotionLevel - 1][i]).Level) {
                                        if(ItemListManager.Instance.GetItemCount(masterData.GearIDs[c.PromotionLevel - 1][i]) > 0 ||
                                           CheckCreateGear(masterData.GearIDs[c.PromotionLevel - 1][i])) {
                                            iconMenuNotification.SetActive(true);
                                            iconCharacterNotification.SetActive(true);
                                            return;
                                        }
                                    }
                                }
                            }
                        }).Subscribe();
                        gcrm.SoulRepository.GetByCharacterId(c.CharacterID).Do(soul => {
                            var haveStone = ItemListManager.Instance.GetItemCount(soul.ID);
                            var needStone = character.EvolutionLevel > 4 ? 0 :
                                character.EvolutionLevel > 3 ? 150 :
                                character.EvolutionLevel > 2 ? 100 :
                                character.EvolutionLevel > 1 ? 50 :
                                character.EvolutionLevel > 0 ? 20 : 0;
                            if(needStone != 0 && haveStone >= needStone) {
                                iconMenuNotification.SetActive(true);
                                iconCharacterNotification.SetActive(true);
                            }
                        }).Subscribe();
                    }).Subscribe();

                }
            }).Subscribe();
        }

        private bool CheckCreateGear(uint id) {
            var recipes = ItemListManager.Instance.GetGearRecipe(id);
            var isCreate = true;
            if(recipes.Count < 1) {
                isCreate = false;
            } else {
                foreach(var recipe in recipes) {
                    if(recipe["num"] > ItemListManager.Instance.GetItemCount(recipe["id"])) {
                        if(ItemListManager.Instance.GetGearRecipe(recipe["id"]) != null) {
                            isCreate = CheckCreateGear(recipe["id"]);
                            if(!isCreate) {break;}
                        } else {
                            isCreate = false;
                            break;
                        }
                    }
                }
            }
            return isCreate;
        }
    }
}
