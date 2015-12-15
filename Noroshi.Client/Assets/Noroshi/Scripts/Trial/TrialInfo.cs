using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Noroshi.Game;

namespace Noroshi.UI {
    public class TrialInfo : MonoBehaviour {
        public class StageData {
            public string Name;
            public int Level = 0;
            public bool IsOpen = true;
            public int Stamina;
            public int Rank = 0;
            public int RemainNum = 0;
            public int[] EnemyList;
            public int[] ItemList;
            public int StageID;
            public uint[] RequireIDList;
        }

        [SerializeField] Text txtTitle;
        [SerializeField] Text txtDescription;
        [SerializeField] Text txtStamina;
        [SerializeField] Text txtRemainChance;
        [SerializeField] GameObject remainContainer;
        [SerializeField] Text txtHaveTicket;
        [SerializeField] Image[] scoreStarList;
        [SerializeField] Sprite starSpriteOn;
        [SerializeField] Sprite starSpriteOff;
        [SerializeField] BtnCaption[] btnEnemyCaptionList;
        [SerializeField] CaptionCharacter captionCharacter;
        [SerializeField] BtnCaption[] btnItemCaptionList;
        [SerializeField] CaptionItem captionItem;
        [SerializeField] BtnCommon overlay;
        [SerializeField] BtnCommon btnClose;
        [SerializeField] BtnCommon btnFight;
        [SerializeField] BtnCommon btnBattleCharacterEdit;
        [SerializeField] CharacterStatusIcon[] battleCharacterList;
        [SerializeField] AlertModal noStaminaAlert;

        public Subject<int> OnSelectBattle = new Subject<int>();
        public Subject<bool> OnCloseStageInfo = new Subject<bool>();
        public Subject<int> OnEditCharacter = new Subject<int>();

        private int _stamina;
        private bool _isModalOpen = false;
        private bool _isCaptionOpen = false;
        private List<CaptionItem.ItemInfo> itemInfoList;


        private void Start() {
            foreach(var btnItem in btnItemCaptionList) {
                var pos = btnItem.transform.localPosition;
                btnItem.OnTouchBtn.Subscribe(index => {
                    if(_isCaptionOpen) {return;}
                    _isCaptionOpen = true;
                    captionItem.ShowCaption(itemInfoList[index], pos);
                }).AddTo(btnItem);
                btnItem.OnReleaseBtn.Subscribe(index => {
                    _isCaptionOpen = false;
                    captionItem.HideCaption();
                }).AddTo(btnItem);
            }

            overlay.OnClickedBtn.Subscribe(_ => {
                CloseStageInfo();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnClose.OnClickedBtn.Subscribe(id => {
                CloseStageInfo();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnFight.OnClickedBtn.Subscribe(stageID => {
                if(PlayerInfo.Instance.CurrentStamina < _stamina) {
                    noStaminaAlert.OnOpen();
                    return;
                }
                OnSelectBattle.OnNext(stageID);
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });

            btnBattleCharacterEdit.OnClickedBtn.Subscribe(stageID => {
                OnEditCharacter.OnNext(stageID);
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });
        }
        
        private void SetStageInfo(StageData data) {
            txtTitle.text = data.Name.ToString();
//            txtDescription.text = data.Description.ToString();
            txtStamina.text = data.Stamina.ToString();
            if((int)data.RemainNum > 1) {
                txtRemainChance.text = data.RemainNum.ToString();
                remainContainer.SetActive(true);
            } else {
                remainContainer.SetActive(false);
            }
//            txtHaveTicket.text = data["haveTicket"].ToString();
            for(int i = 0; i < scoreStarList.Length; i++) {
                if(i < data.Rank) {
                    scoreStarList[i].sprite = starSpriteOn;
                } else {
                    scoreStarList[i].sprite = starSpriteOff;
                }
            }

            btnBattleCharacterEdit.id = (int)data.StageID;
            btnFight.id = (int)data.StageID;
        }
        
        private void SetEnemyImg(int[] enemyIDList) {
            foreach (var btn in btnEnemyCaptionList) {
                btn.gameObject.SetActive(false);
            }
            var l = enemyIDList == null ? 5 : enemyIDList.Length;
//            var enemyIDs = battleData.Waves[battleData.Waves.Length - 1].EnemyCharacterIDs;

            for(int i = 0; i < l; i++) {
//                var isBoss = (bool)data[id]["EnemyList"][i]["isBoss"];
//                var enemyID = (int)data[id]["EnemyList"][i];
//                var sprite = Resources.Load<Sprite>("Sprites/Chara/thumb_chara" + enemyID);
//                btnEnemyCaptionList[i].SetImage(sprite);
                if(i == l - 1) {
                    var sprite = Resources.Load<Sprite>("Sprites/Chara/boss");
                    btnEnemyCaptionList[i].SetImage(sprite);
                    btnEnemyCaptionList[i].transform.SetAsFirstSibling();
//                    btnEnemyCaptionList[i].transform.localScale = new Vector3(-1.5f, 1.5f);
                    TweenXY.Add(btnEnemyCaptionList[i].gameObject, 0.4f, new Vector2(344, -34))
                        .From(new Vector2(0, -700)).Delay((l - 1) * 0.06f + 0.4f).EaseInOutBackWith(0.75f);
                } else {
                    var x = i * 130 - ((int)(i / 2) * 340) + 136;
                    var y = (int)(i / 2) * -100 - 144;
                    var sprite = Resources.Load<Sprite>("Sprites/Chara/enemy");
                    btnEnemyCaptionList[i].SetImage(sprite);
                    btnEnemyCaptionList[i].transform.SetAsLastSibling();
//                    btnEnemyCaptionList[i].transform.localScale = new Vector3(-1, 1);
                    TweenXY.Add(btnEnemyCaptionList[i].gameObject, 0.5f, new Vector2(x, y))
                        .From(new Vector2(0, -600)).Delay(i * 0.06f).EaseInQuartic();
                }
            }
        }

        private void SetItemImg(int[] itemIDList) {
            var gcrm = GlobalContainer.RepositoryManager;
            var list = "";
            var itemListCount = 0;

            foreach (var btn in btnItemCaptionList) {
                btn.gameObject.SetActive(false);
            }
            if(itemIDList == null) {return;}
            for(int i = 0, l = Mathf.Min(itemIDList.Length, btnItemCaptionList.Length); i < l; i++) {
                var sprite = Resources.Load<Sprite>("Item/" + itemIDList[i]);
                btnItemCaptionList[i].SetImage(sprite);
            }
        }

        private CharacterStatusIcon.CharacterData SetCharacterStatus(CharacterStatus characterStatus) {
            var characterData = new CharacterStatusIcon.CharacterData();
            
            characterData.CharacterID = characterStatus.CharacterID;
            characterData.Level = characterStatus.Level;
            characterData.EvolutionLevel = characterStatus.EvolutionLevel;
            characterData.PromotionLevel = characterStatus.PromotionLevel;
            characterData.IsDeca = characterStatus.TagSet.IsDeca;
            return characterData;
        }

        public void SetBattleCharacterIcon(uint[] idList) {
            var gcrm = GlobalContainer.RepositoryManager;
            var ids = BattleCharacterSelect.Instance.GetPlayerCharacterId(idList);
            for(int i = 0, l = battleCharacterList.Length; i < l; i++) {
                if(i < ids.Length) {
                    int n = i;
                    gcrm.LoadCharacterStatusByPlayerCharacterID(ids[n]).Do(characterStatus => {
                        var characterData = SetCharacterStatus(characterStatus);
                        battleCharacterList[n].SetInfo(characterData);
                    }).Subscribe();
                    battleCharacterList[i].gameObject.SetActive(true);
                } else {
                    battleCharacterList[i].gameObject.SetActive(false);
                }
            }
        }

        public void OpenStageInfo(StageData stageData) {
            if(_isModalOpen) {return;}
            _stamina = stageData.Stamina;
            SetStageInfo(stageData);
            SetEnemyImg(stageData.EnemyList);
            SetItemImg(stageData.ItemList);
            gameObject.SetActive(true);
            TweenA.Add(gameObject, 0.3f, 1);
        }

        public void CloseStageInfo() {
            _isModalOpen = false;
            OnCloseStageInfo.OnNext(true);
            TweenA.Add(gameObject, 0.3f, 0).Then(() => {
                gameObject.SetActive(false);}
            );
        }
    }
}
