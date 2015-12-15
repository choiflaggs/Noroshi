using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using LitJson;
using Noroshi.Game;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.UI {
    public class StageInfo : MonoBehaviour {
        public class StageData {
            public string Name;
            public int Level = 0;
            public bool IsOpen = true;
            public int Stamina;
            public int Rank = 0;
            public int RemainNum = 0;
            public BattleCharacter[] EnemyList;
            public PossessionObject[] ItemList;
            public int BattleID;
            public int StageID;
            public int ChapterIndex;
            public int EpisodeIndex;
            public int StageIndex;
            public uint[] RequireIDList;
            public uint[] CPUIDList;
        }

        [SerializeField] Text txtTitle;
        [SerializeField] Text txtChapterIndex;
        [SerializeField] Text txtEpisodeIndex;
        [SerializeField] Text txtStageIndex;
        [SerializeField] Text txtStamina;
        [SerializeField] Text txtRemainChance;
        [SerializeField] GameObject stageInfoPanel;
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
        [SerializeField] GameObject quickBattle;
        [SerializeField] BtnCommon btnBattle;
        [SerializeField] BtnCommon btnQuickBattle;
        [SerializeField] BtnCommon btnOpenStageInfo;
        [SerializeField] BtnCommon btnCloseStageInfo;

        public Subject<int> OnOpenStageInfo = new Subject<int>();
        public Subject<int> OnEditCharacter = new Subject<int>();

        private int _stamina;
        private int crntIndex;
        private bool _isCaptionOpen = false;
        private List<CaptionCharacter.CharacterInfo> enemyInfoList;
        private List<CaptionItem.ItemInfo> itemInfoList;
        private float defaultPositionY;
        private float quickBattlePositionY;


        private void Start() {
            foreach(var btnEnemy in btnEnemyCaptionList) {
                btnEnemy.OnTouchBtn.Subscribe(index => {
                    var pos = btnEnemyCaptionList[index].transform.position;
                    if(_isCaptionOpen) {return;}
                    _isCaptionOpen = true;
                    captionCharacter.ShowCaption(enemyInfoList[index], pos);
                }).AddTo(btnEnemy);
                btnEnemy.OnReleaseBtn.Subscribe(index => {
                    _isCaptionOpen = false;
                    captionCharacter.HideCaption();
                }).AddTo(btnEnemy);
            }
            foreach(var btnItem in btnItemCaptionList) {
                btnItem.OnTouchBtn.Subscribe(index => {
                    var pos = btnItemCaptionList[index].transform.position;
                    if(_isCaptionOpen) {return;}
                    _isCaptionOpen = true;
                    captionItem.ShowCaption(itemInfoList[index], pos);
                }).AddTo(btnItem);
                btnItem.OnReleaseBtn.Subscribe(index => {
                    _isCaptionOpen = false;
                    captionItem.HideCaption();
                }).AddTo(btnItem);
            }

            defaultPositionY = stageInfoPanel.transform.localPosition.y;
            quickBattlePositionY = quickBattle.transform.localPosition.y;

            overlay.OnClickedBtn.Subscribe(_ => {
                TweenA.Add(overlay.gameObject, 0.25f, 0);
                TweenY.Add(quickBattle, 0.25f, quickBattlePositionY - 40).EaseOutCubic();
                TweenA.Add(quickBattle, 0.25f, 0).EaseOutCubic().Then(() => {
                    quickBattle.SetActive(false);
                    overlay.gameObject.SetActive(false);
                });
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });

            btnQuickBattle.OnClickedBtn.Subscribe(_ => {
                overlay.gameObject.SetActive(true);
                quickBattle.SetActive(true);
                TweenA.Add(overlay.gameObject, 0.1f, 0.7f).EaseOutCubic();
                TweenA.Add(quickBattle, 0.2f, 1).From(0).EaseOutCubic();
                TweenY.Add(quickBattle, 0.2f, quickBattlePositionY).From(quickBattlePositionY - 40).EaseOutCubic();
            });

            btnBattle.OnClickedBtn.Subscribe(stageID => {
                OnEditCharacter.OnNext(stageID);
                SoundController.Instance.PlaySE(SoundController.SEKeys.DECIDE);
            });

            btnOpenStageInfo.OnClickedBtn.Subscribe(_ => {
                OpenStageInfo();
            });

            btnCloseStageInfo.OnClickedBtn.Subscribe(_ => {
                CloseStageInfo();
            });
            gameObject.SetActive(false);
        }

        private void SetEnemyImg(BattleCharacter[] enemyList) {
            var gcrm = GlobalContainer.RepositoryManager;

            enemyInfoList = new List<CaptionCharacter.CharacterInfo>();
            foreach (var btn in btnEnemyCaptionList) {
                btn.gameObject.SetActive(false);
            }

            for(int i = 0, l = Mathf.Min(enemyList.Length, btnEnemyCaptionList.Length); i < l; i++) {
                var enemyInfo = new CaptionCharacter.CharacterInfo();
                gcrm.CharacterRepository.Get(enemyList[i].CharacterID).Do(masterData => {
                    enemyInfo.Name = GlobalContainer.LocalizationManager.GetText(masterData.TextKey + ".Name");
                }).Subscribe();
                enemyInfo.Lv = enemyList[i].Level;
                enemyInfo.PromotionLevel = enemyList[i].PromotionLevel;
                enemyInfo.ID = enemyList[i].CharacterID;
                enemyInfo.IsBoss = enemyList[i].IsBoss;
//                enemyInfo.Description = enemyList[i].Description;
                enemyInfo.Index = i;
                enemyInfoList.Add(enemyInfo);
                enemyInfoList = enemyInfoList.OrderBy(c => c.Index).ToList();

                var sprite = Resources.Load<Sprite>(string.Format("Character/{0}/thumb_1", enemyInfo.ID));
                btnEnemyCaptionList[i].SetImage(sprite);
                if(enemyList[i].IsBoss) {

                } else {
                   
                }
            }
        }

        private void SetItemImg(PossessionObject[] itemList) {
            itemInfoList = new List<CaptionItem.ItemInfo>();
            foreach (var btn in btnItemCaptionList) {
                btn.gameObject.SetActive(false);
            }
            if(itemList == null || itemList.Length < 1) {return;}
            for(int i = 0, l = Mathf.Min(itemList.Length, btnItemCaptionList.Length); i < l; i++) {
                var itemInfo = new CaptionItem.ItemInfo();
                var sprite = Resources.Load<Sprite>("Item/" + itemList[i].ID);
                btnItemCaptionList[i].SetImage(sprite);

                itemInfo.Name = GlobalContainer.LocalizationManager.GetText(itemList[i].Name + ".Name");
                itemInfo.HaveNum = itemList[i].PossessingNum;
//                itemInfo.NeedLevel = gearData != null ? (uint)gearData.Level : 1;
//                itemInfo.Price = itemData.Price;
                itemInfo.ID = itemList[i].ID;
//                itemInfo.Description = itemData.FlavorText;
                itemInfo.Index = i;
                itemInfoList.Add(itemInfo);
                itemInfoList = itemInfoList.OrderBy(c => c.Index).ToList();
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

        public void SetStageInfo(StageData data) {
            txtTitle.text = data.Name.ToString();
            txtChapterIndex.text = (data.ChapterIndex + 1).ToString();
            txtEpisodeIndex.text = (data.EpisodeIndex + 1).ToString();
            txtStageIndex.text = (data.StageIndex + 1).ToString();
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
            _stamina = data.Stamina;
            SetEnemyImg(data.EnemyList);
            SetItemImg(data.ItemList);
            btnBattle.id = (int)data.StageID;
            crntIndex = data.StageIndex;
            OpenStageInfo();
        }

        public void OpenStageInfo() {
            btnCloseStageInfo.gameObject.SetActive(true);
            OnOpenStageInfo.OnNext(crntIndex);
            TweenA.Add(btnCloseStageInfo.gameObject, 0.2f, 1);
            TweenA.Add(btnOpenStageInfo.gameObject, 0.2f, 0).Then(() => {
                btnOpenStageInfo.gameObject.SetActive(false);
            });
            TweenY.Add(stageInfoPanel, 0.2f, defaultPositionY).EaseOutCubic();
        }

        public void CloseStageInfo() {
            btnOpenStageInfo.gameObject.SetActive(true);
            TweenA.Add(btnOpenStageInfo.gameObject, 0.2f, 1);
            TweenA.Add(btnCloseStageInfo.gameObject, 0.2f, 0).Then(() => {
                btnCloseStageInfo.gameObject.SetActive(false);
            });
            TweenY.Add(stageInfoPanel, 0.2f, defaultPositionY - 258).EaseOutCubic();
        }
    }
}
