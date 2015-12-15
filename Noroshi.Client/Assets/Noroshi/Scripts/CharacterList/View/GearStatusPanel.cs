using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using Noroshi.UI;

namespace Noroshi.CharacterList {
    public class GearStatusPanel : MonoBehaviour {
        [SerializeField] Text txtGearName;
        [SerializeField] Image imgGear;
        [SerializeField] Text txtEnchant;
        [SerializeField] Text txtHaveNum;
        [SerializeField] Text txtNeedLevel;
        [SerializeField] GameObject strengthWrapper;
        [SerializeField] GameObject intellectWrapper;
        [SerializeField] GameObject agilityWrapper;
        [SerializeField] GameObject hpWrapper;
        [SerializeField] GameObject physicalAttackWrapper;
        [SerializeField] GameObject magicPowerWrapper;
        [SerializeField] GameObject armorWrapper;
        [SerializeField] GameObject magicResistanceWrapper;
        [SerializeField] GameObject physicalCritWrapper;
        [SerializeField] GameObject magicCritWrapper;
        [SerializeField] GameObject hpRegenWrapper;
        [SerializeField] GameObject energyRegenWrapper;
        [SerializeField] GameObject accuracyWrapper;
        [SerializeField] GameObject dodgeWrapper;
        [SerializeField] GameObject armorPenetrationWrapper;
        [SerializeField] GameObject ignoreMagicResistanceWrapper;
        [SerializeField] GameObject lifeStealRatingWrapper;
        [SerializeField] GameObject improveHealingsWrapper;
        [SerializeField] Text txtStrength;
        [SerializeField] Text txtIntellect;
        [SerializeField] Text txtAgility;
        [SerializeField] Text txtHp;
        [SerializeField] Text txtPhysicalAttack;
        [SerializeField] Text txtMagicPower;
        [SerializeField] Text txtArmor;
        [SerializeField] Text txtMagicResistance;
        [SerializeField] Text txtPhysicalCrit;
        [SerializeField] Text txtMagicCrit;
        [SerializeField] Text txtHPRegen;
        [SerializeField] Text txtEnergyRegen;
        [SerializeField] Text txtAccuracy;
        [SerializeField] Text txtDodge;
        [SerializeField] Text txtArmorPenetration;
        [SerializeField] Text txtIgnoreMagicResistance;
        [SerializeField] Text txtLifeStealRating;
        [SerializeField] Text txtImproveHealings;
        [SerializeField] BtnCommon btnEquip;
        [SerializeField] BtnCommon btnEnchant;
        [SerializeField] BtnCommon btnRecipe;

        public Subject<uint> OnEquip = new Subject<uint>();
        public Subject<bool> OnOpenRecipe = new Subject<bool>();

        private Core.WebApi.Response.Gear gearData;

        private void Start() {
            btnRecipe.OnClickedBtn.Subscribe(_ => {
                OnOpenRecipe.OnNext(true);
            });
            btnRecipe.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
            
            btnEquip.OnClickedBtn.Subscribe(_ => {
                ItemListManager.Instance.ChangeItemCount(gearData.ID, -1);
                OnEquip.OnNext(gearData.ID);
            });
            btnEquip.OnPlaySE.Subscribe(_ => {
                SoundController.Instance.PlaySE(SoundController.SEKeys.EQUIP);
            });
        }

        private void SetGearData(Core.WebApi.Response.Gear gear) {
            if(gear.Strength != 0) {
                txtStrength.text = gear.Strength.ToString();
                strengthWrapper.SetActive(true);
            } else {strengthWrapper.SetActive(false);}
            if(gear.Intellect != 0) {
                txtIntellect.text = gear.Intellect.ToString();
                intellectWrapper.SetActive(true);
            } else {intellectWrapper.SetActive(false);}
            if(gear.Agility != 0) {
                txtAgility.text = gear.Agility.ToString();
                agilityWrapper.SetActive(true);
            } else {agilityWrapper.SetActive(false);}
            if(gear.HP != 0) {
                txtHp.text = gear.HP.ToString();
                hpWrapper.SetActive(true);
            } else {hpWrapper.SetActive(false);}
            if(gear.PhysicalAttack != 0) {
                txtPhysicalAttack.text = gear.PhysicalAttack.ToString();
                physicalAttackWrapper.SetActive(true);
            } else {physicalAttackWrapper.SetActive(false);}
            if(gear.MagicPower != 0) {
                txtMagicPower.text = gear.MagicPower.ToString();
                magicPowerWrapper.SetActive(true);
            } else {magicPowerWrapper.SetActive(false);}
            if(gear.Armor != 0) {
                txtArmor.text = gear.Armor.ToString();
                armorWrapper.SetActive(true);
            } else {armorWrapper.SetActive(false);}
            if(gear.MagicResistance != 0) {
                txtMagicResistance.text = gear.MagicResistance.ToString();
                magicResistanceWrapper.SetActive(true);
            } else {magicResistanceWrapper.SetActive(false);}
            if(gear.PhysicalCrit != 0) {
                txtPhysicalCrit.text = gear.PhysicalCrit.ToString();
                physicalCritWrapper.SetActive(true);
            } else {physicalCritWrapper.SetActive(false);}
            if(gear.MagicCrit != 0) {
                txtMagicCrit.text = gear.MagicCrit.ToString();
                magicCritWrapper.SetActive(true);
            } else {magicCritWrapper.SetActive(false);}
            if(gear.HPRegen != 0) {
                txtHPRegen.text = gear.HPRegen.ToString();
                hpRegenWrapper.SetActive(true);
            } else {hpRegenWrapper.SetActive(false);}
            if(gear.EnergyRegen != 0) {
                txtEnergyRegen.text = gear.EnergyRegen.ToString();
                energyRegenWrapper.SetActive(true);
            } else {energyRegenWrapper.SetActive(false);}
            if(gear.Accuracy != 0) {
                txtAccuracy.text = gear.Accuracy.ToString();
                accuracyWrapper.SetActive(true);
            } else {accuracyWrapper.SetActive(false);}
            if(gear.Dodge != 0) {
                txtDodge.text = gear.Dodge.ToString();
                dodgeWrapper.SetActive(true);
            } else {dodgeWrapper.SetActive(false);}
            if(gear.ArmorPenetration != 0) {
                txtArmorPenetration.text = gear.ArmorPenetration.ToString();
                armorPenetrationWrapper.SetActive(true);
            } else {armorPenetrationWrapper.SetActive(false);}
            if(gear.IgnoreMagicResistance != 0) {
                txtIgnoreMagicResistance.text = gear.IgnoreMagicResistance.ToString();
                ignoreMagicResistanceWrapper.SetActive(true);
            } else {ignoreMagicResistanceWrapper.SetActive(false);}
            if(gear.LifeStealRating != 0) {
                txtLifeStealRating.text = gear.LifeStealRating.ToString();
                lifeStealRatingWrapper.SetActive(true);
            } else {lifeStealRatingWrapper.SetActive(false);}
            if(gear.ImproveHealings != 0) {
                txtImproveHealings.text = gear.ImproveHealings.ToString();
                improveHealingsWrapper.SetActive(true);
            } else {improveHealingsWrapper.SetActive(false);}
        }

        public void SetGearStatusPanel(Core.WebApi.Response.Gear data, int state, int lv) {
            var gcrm = GlobalContainer.RepositoryManager;
            var haveNum = ItemListManager.Instance.GetItemCount(data.ID);
            gearData = data;
            gcrm.GearRepository.Get(gearData.ID).Do(gear => {
                txtGearName.text = GlobalContainer.LocalizationManager.GetText(gear.TextKey + ".Name");
            }).Subscribe();
            imgGear.sprite = Resources.Load<Sprite>("Item/" + gearData.ID);
            txtHaveNum.text = haveNum.ToString();
            if(haveNum > 0) {
                txtHaveNum.color = Constant.TEXT_COLOR_NORMAL_DARK;
            } else {
                txtHaveNum.color = Constant.TEXT_COLOR_NEGATIVE;
            }
            txtNeedLevel.text = gearData.Level.ToString();
            if(gearData.Level > lv) {
                txtNeedLevel.color = Constant.TEXT_COLOR_NEGATIVE;
            } else {
                txtNeedLevel.color = Constant.TEXT_COLOR_NORMAL_DARK;
            }
            SetGearData(gearData);
            if(state > 0) {
                btnEnchant.gameObject.SetActive(true);
                btnEquip.gameObject.SetActive(false);
            } else {
                btnEnchant.gameObject.SetActive(false);
                btnEquip.gameObject.SetActive(true);
                if (state == -2) {
                    btnEquip.SetEnable(true);
                } else {
                    btnEquip.SetEnable(false);
                }
            }
        }

        public void CloseGearStatus() {
            gameObject.transform.localPosition = new Vector3(0, -22, 0);
        }
    }
}
