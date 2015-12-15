using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.WebApi.Response.Possession;
using Noroshi.Core.Game.Possession;

namespace Noroshi.UI {
    public class GachaResult : MonoBehaviour {
        [SerializeField] GameObject resultTreasure;
        [SerializeField] GachaResultItem resultItem;
        [SerializeField] GachaResultItem[] resultItemList;
        [SerializeField] BtnCommon btnOk;
        [SerializeField] BtnCommon btnAgain;

        public Subject<int> OnGachaAgain = new Subject<int>();

        private List<uint> newCharacterIDList;

        private void Start() {
            btnOk.OnClickedBtn.Subscribe(_ => {
                CloseGachaResult();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnAgain.OnClickedBtn.Subscribe(id => {
                CloseGachaResult();
                OnGachaAgain.OnNext(id);
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });
        }

        private void CloseGachaResult() {
            resultItem.Disapper();
            foreach(var item in resultItemList) {
                item.Disapper();
            }
            btnOk.gameObject.SetActive(false);
            btnAgain.gameObject.SetActive(false);
            TweenNull.Add(gameObject, 0.01f).Then(() => {
                gameObject.SetActive(false);
            });
        }

        public void StartAnimation(int type) {
            btnAgain.id = type;
            gameObject.SetActive(true);
            TweenA.Add(resultTreasure, 0.05f, 1).From(0).Delay(0.01f);
            TweenS.Add(resultTreasure, 0.01f, 2.5f).Then(() => {
                TweenS.Add(resultTreasure, 0.3f, 1.0f).Delay(0.8f).EaseInCubic();
            });
            TweenY.Add(resultTreasure, 0.01f, 0).Then(() => {
                TweenY.Add(resultTreasure, 0.3f, 220).Delay(0.8f).EaseInCubic();
            });
            if(type == 0) {
                resultItem.Move(1.2f);
                TweenNull.Add(gameObject, 1.5f).Then(() => {
                    btnOk.gameObject.SetActive(true);
                    if(newCharacterIDList.Count > 0) {
                        foreach(var charaID in newCharacterIDList) {
                            var getModal = Instantiate(Resources.Load<GetCharacterModal>("UI/GetCharacterModal"));
                            getModal.OpenModal(charaID);
                        }
                    }
//                    btnAgain.gameObject.SetActive(true);
                });
            } else {
                for(int i = 0, l = resultItemList.Length; i < l; i++) {
                    float d = (float)i * 0.14f + 1.3f;
                    resultItemList[i].Move(d);
                }
                TweenNull.Add(gameObject, 2.9f).Then(() => {
                    btnOk.gameObject.SetActive(true);
                    if(newCharacterIDList.Count > 0) {
                        var getModal = Instantiate(Resources.Load<GetCharacterModal>("UI/GetCharacterModal"));
                        getModal.OpenModal(newCharacterIDList[0]);
                    }
//                    btnAgain.gameObject.SetActive(true);
                });
            }
        }

        public void SetGachaResult(PossessionObject[] results) {
            newCharacterIDList = new List<uint>();
            if(results.Length > 1) {
                for(int i = 0, l = resultItemList.Length; i < l; i++) {
                    Sprite sprite = null;
                    string name = null;
                    if(results[i] != null) {
                        if(results[i].Category == (byte)PossessionCategory.Character) {
                            sprite = Resources.Load<Sprite>("Character/" + results[i].ID + "/thumb_1");
                            name = GlobalContainer.LocalizationManager.GetText(results[i].Name + ".Name");
                            var pcID = BattleCharacterSelect.Instance.GetPlayerCharacterId(new uint[]{results[i].ID});
                            if(pcID.Length < 1) {
                                newCharacterIDList.Add(results[i].ID);
                            }
                        } else {
                            sprite = Resources.Load<Sprite>("Item/" + results[i].ID);
                            name = results[i].Name;
                        }
                        resultItemList[i].gameObject.SetActive(true);
                        resultItemList[i].SetItem(sprite, name);
                    } else {
                        resultItemList[i].gameObject.SetActive(false);
                    }
                }
                StartAnimation(1);
            } else {
                Sprite sprite = null;
                string name = null;
                if(results[0].Category == (byte)PossessionCategory.Character) {
                    sprite = Resources.Load<Sprite>("Character/" + results[0].ID + "/thumb_1");
                    name = GlobalContainer.LocalizationManager.GetText(results[0].Name + ".Name");
                    var pcID = BattleCharacterSelect.Instance.GetPlayerCharacterId(new uint[]{results[0].ID});
                    if(pcID.Length < 1) {
                        newCharacterIDList.Add(results[0].ID);
                    }
                } else {
                    sprite = Resources.Load<Sprite>("Item/" + results[0].ID);
                    name = results[0].Name;
                }
                resultItem.SetItem(sprite, name);
                StartAnimation(0);
            }
        }
    }
}
