using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Noroshi.Game;
using Noroshi.UI;
using NoroshiDebug.Repositories.Server;
using UniRx;

namespace Noroshi.NoroshiDebug {
    public class DebugCharacterDetail : MonoBehaviour {
        [SerializeField] Image imgChara;
        [SerializeField] Text txtCharaName;
        [SerializeField] Text txtLv;
        [SerializeField] Text txtRare;
        [SerializeField] Text txtPromotion;
        [SerializeField] Text[] txtSkillList;
        [SerializeField] BtnCommon btnCloseDetail;
        [SerializeField] BtnCommon btnIncreaseLevel;
        [SerializeField] BtnCommon btnDecreaseLevel;
        [SerializeField] BtnCommon btnIncreaseRarity;
        [SerializeField] BtnCommon btnDecreaseRarity;
        [SerializeField] BtnCommon btnIncreasePromotion;
        [SerializeField] BtnCommon btnDecreasePromotion;
        [SerializeField] BtnCommon btnEquipAll;
        [SerializeField] BtnCommon btnRemoveEquip;
        [SerializeField] BtnCommon[] btnIncreaseSkillList;
        [SerializeField] BtnCommon[] btnDecreaseSkillList;
        [SerializeField] GameObject content;
        [SerializeField] AlertModal equipAlert;
        [SerializeField] AlertModal removeAlert;
        [SerializeField] AlertModal limitAlert;
        [SerializeField] GameObject processing;

        public Subject<int> OnChangeLevel = new Subject<int>();
        public Subject<int> OnChangeRarity = new Subject<int>();
        public Subject<int> OnChangePromotion = new Subject<int>();
        public Subject<Dictionary<string, int>> OnChangeSkill = new Subject<Dictionary<string, int>>();

        private CharacterStatus characterStatus;
        private uint playerCharacterID;
        private byte initialEvolutionLv;
        private PlayerCharacterDebugReporitosy _reporitosy = new PlayerCharacterDebugReporitosy();

        private void Start()
        {
            btnCloseDetail.OnClickedBtn.Subscribe(id => {
                TweenA.Add(content, 0.2f, 0).EaseOutCubic().Then(() => {
                    gameObject.SetActive(false);
                });
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            btnIncreaseLevel.OnClickedBtn.Subscribe(_ => {
                ChangeLevel(1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.LEVEL_UP);
            });
            btnDecreaseLevel.OnClickedBtn.Subscribe(_ => {
                ChangeLevel(-1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            btnIncreaseRarity.OnClickedBtn.Subscribe(_ => {
                ChangeEvolution(1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.EVOLUTION);
            });
            btnDecreaseRarity.OnClickedBtn.Subscribe(_ => {
                ChangeEvolution(-1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            btnIncreasePromotion.OnClickedBtn.Subscribe(_ => {
                ChangePromotion(1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.UPGRADE);
            });
            btnDecreasePromotion.OnClickedBtn.Subscribe(_ => {
                ChangePromotion(-1);
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            btnEquipAll.OnClickedBtn.Subscribe(_ => {
                EquipAll();
                SoundController.Instance.PlaySE(SoundController.SEKeys.EQUIP);
            });
            btnRemoveEquip.OnClickedBtn.Subscribe(_ => {
                RemoveEquip();
                SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
            });
            for(int i = 0, l = btnIncreaseSkillList.Length; i < l; i++) {
                var index = i + 1;
                btnIncreaseSkillList[i].OnClickedBtn.Subscribe(_ => {
                    ChangeSkill(index, 1);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.STATUS_UP);
                });
                btnDecreaseSkillList[i].OnClickedBtn.Subscribe(_ => {
                    ChangeSkill(index, -1);
                    SoundController.Instance.PlaySE(SoundController.SEKeys.CANCEL);
                });
            }
        }

        private void ChangeLevel(int value) {
            ushort lvValue = (ushort)(value + int.Parse(txtLv.text));
            if (lvValue < 1 || lvValue > 99) {
                limitAlert.OnOpen();
                return;
            }
            processing.SetActive(true);
            _reporitosy.ChangedLevel(playerCharacterID, lvValue).Do(data => {
                txtLv.text = data.Level.ToString();
                OnChangeLevel.OnNext(data.Level);
                processing.SetActive(false);
           }).Subscribe();
        }

        private void ChangeEvolution(int value) {
            byte evolutionValue = (byte)(value + int.Parse(txtRare.text));
            if(evolutionValue < initialEvolutionLv || evolutionValue > 5) {
                limitAlert.OnOpen();
                return;
            }
            processing.SetActive(true);
            _reporitosy.ChangeEvolutionLevel(playerCharacterID, evolutionValue).Do(data => {
                txtRare.text = data.EvolutionLevel.ToString();
                OnChangeRarity.OnNext(data.EvolutionLevel);
                processing.SetActive(false);
            }).Subscribe();
        }

        private void ChangePromotion(int value) {
            byte promotionValue = (byte)(value + int.Parse(txtPromotion.text));
            if (promotionValue < 1 || promotionValue > 12) {
                limitAlert.OnOpen();
                return;
            }
            processing.SetActive(true);
            _reporitosy.ChangePromotionLevel(playerCharacterID, promotionValue).Do(data => {
                txtPromotion.text = data.PromotionLevel.ToString();
                OnChangePromotion.OnNext(data.PromotionLevel);
                processing.SetActive(false);
            }).Subscribe();
        }

        private void EquipAll() {
            processing.SetActive(true);
            _reporitosy.AllEquipGear(playerCharacterID).Do(data => {
                processing.SetActive(false);
                equipAlert.OnOpen();
            }).Subscribe();
        }

        private void RemoveEquip() {
            processing.SetActive(true);
            _reporitosy.RemoveEquipGear(playerCharacterID).Do(data => {
                processing.SetActive(false);
                removeAlert.OnOpen();
            }).Subscribe();
        }

        private void ChangeSkill(int index, int value) {
            ushort skillValue = (ushort)(value + int.Parse(txtSkillList[index - 1].text));
            if (skillValue < 1 || skillValue > 99 || skillValue > int.Parse(txtLv.text)) {
                limitAlert.OnOpen();
                return;
            }
            processing.SetActive(true);
            _reporitosy.ChangeActionLevel(playerCharacterID, skillValue, (ushort)index).Do(data => {
                int v = 0;
                switch (index) {
                    case 1:
                        v = data.ActionLevel1;
                        break;
                    case 2:
                        v = data.ActionLevel2;
                        break;
                    case 3:
                        v = data.ActionLevel3;
                        break;
                    case 4:
                        v = data.ActionLevel4;
                        break;
                    case 5:
                        v = data.ActionLevel5;
                        break;
                }

                txtSkillList[index - 1].text = v.ToString();
                OnChangeSkill.OnNext(new Dictionary<string, int>() {
                    {"index", index - 1},
                    {"value", v}
                });
                processing.SetActive(false);
            }).Subscribe();
        }

        public void OpenDetail(DebugCharacter.DebugCharacterStatus status) {
            GlobalContainer.RepositoryManager.CharacterRepository.Get(status.id).Do(masterData => {
                txtCharaName.text = GlobalContainer.LocalizationManager.GetText(masterData.TextKey + ".Name");
                initialEvolutionLv = masterData.InitialEvolutionLevel;
            }).Subscribe();
            playerCharacterID = status.playerCharacterID;
            imgChara.sprite = Resources.Load<Sprite>(
                string.Format("Character/{0}/thumb_1", status.id)
            );
            txtLv.text = status.lv.ToString();
            txtRare.text = status.rare.ToString();
            txtPromotion.text = status.promotion.ToString();

            for(int i = 0, l = status.skillLv.Count; i < l; i++) {
                if (txtSkillList[i] == null) { return; }
                txtSkillList[i].text = status.skillLv[i].ToString();
            }
            gameObject.SetActive(true);
            TweenA.Add(content, 0.5f, 1).EaseOutQuart();
        }
    }
}
