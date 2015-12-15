using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ItemDetail : MonoBehaviour {
        [SerializeField] Text txtName;
        [SerializeField] Image imgItem;
        [SerializeField] Text description;
        [SerializeField] Text haveNum;
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

        private void Start() {

        }

        private void SetGearData(Core.WebApi.Response.Gear gear = null) {
            if(gear != null && gear.Strength != 0) {
                txtStrength.text = gear.Strength.ToString();
                strengthWrapper.SetActive(true);
            } else {strengthWrapper.SetActive(false);}
            if(gear != null && gear.Intellect != 0) {
                txtIntellect.text = gear.Intellect.ToString();
                intellectWrapper.SetActive(true);
            } else {intellectWrapper.SetActive(false);}
            if(gear != null && gear.Agility != 0) {
                txtAgility.text = gear.Agility.ToString();
                agilityWrapper.SetActive(true);
            } else {agilityWrapper.SetActive(false);}
            if(gear != null && gear.HP != 0) {
                txtHp.text = gear.HP.ToString();
                hpWrapper.SetActive(true);
            } else {hpWrapper.SetActive(false);}
            if(gear != null && gear.PhysicalAttack != 0) {
                txtPhysicalAttack.text = gear.PhysicalAttack.ToString();
                physicalAttackWrapper.SetActive(true);
            } else {physicalAttackWrapper.SetActive(false);}
            if(gear != null && gear.MagicPower != 0) {
                txtMagicPower.text = gear.MagicPower.ToString();
                magicPowerWrapper.SetActive(true);
            } else {magicPowerWrapper.SetActive(false);}
            if(gear != null && gear.Armor != 0) {
                txtArmor.text = gear.Armor.ToString();
                armorWrapper.SetActive(true);
            } else {armorWrapper.SetActive(false);}
            if(gear != null && gear.MagicResistance != 0) {
                txtMagicResistance.text = gear.MagicResistance.ToString();
                magicResistanceWrapper.SetActive(true);
            } else {magicResistanceWrapper.SetActive(false);}
            if(gear != null && gear.PhysicalCrit != 0) {
                txtPhysicalCrit.text = gear.PhysicalCrit.ToString();
                physicalCritWrapper.SetActive(true);
            } else {physicalCritWrapper.SetActive(false);}
            if(gear != null && gear.MagicCrit != 0) {
                txtMagicCrit.text = gear.MagicCrit.ToString();
                magicCritWrapper.SetActive(true);
            } else {magicCritWrapper.SetActive(false);}
            if(gear != null && gear.HPRegen != 0) {
                txtHPRegen.text = gear.HPRegen.ToString();
                hpRegenWrapper.SetActive(true);
            } else {hpRegenWrapper.SetActive(false);}
            if(gear != null && gear.EnergyRegen != 0) {
                txtEnergyRegen.text = gear.EnergyRegen.ToString();
                energyRegenWrapper.SetActive(true);
            } else {energyRegenWrapper.SetActive(false);}
            if(gear != null && gear.Accuracy != 0) {
                txtAccuracy.text = gear.Accuracy.ToString();
                accuracyWrapper.SetActive(true);
            } else {accuracyWrapper.SetActive(false);}
            if(gear != null && gear.Dodge != 0) {
                txtDodge.text = gear.Dodge.ToString();
                dodgeWrapper.SetActive(true);
            } else {dodgeWrapper.SetActive(false);}
            if(gear != null && gear.ArmorPenetration != 0) {
                txtArmorPenetration.text = gear.ArmorPenetration.ToString();
                armorPenetrationWrapper.SetActive(true);
            } else {armorPenetrationWrapper.SetActive(false);}
            if(gear != null && gear.IgnoreMagicResistance != 0) {
                txtIgnoreMagicResistance.text = gear.IgnoreMagicResistance.ToString();
                ignoreMagicResistanceWrapper.SetActive(true);
            } else {ignoreMagicResistanceWrapper.SetActive(false);}
            if(gear != null && gear.LifeStealRating != 0) {
                txtLifeStealRating.text = gear.LifeStealRating.ToString();
                lifeStealRatingWrapper.SetActive(true);
            } else {lifeStealRatingWrapper.SetActive(false);}
            if(gear != null && gear.ImproveHealings != 0) {
                txtImproveHealings.text = gear.ImproveHealings.ToString();
                improveHealingsWrapper.SetActive(true);
            } else {improveHealingsWrapper.SetActive(false);}
        }

        public void SetItemDetailData(Core.WebApi.Response.Item itemData, uint num) {
            var gcrm = GlobalContainer.RepositoryManager;
            gameObject.SetActive(false);
            txtName.text = GlobalContainer.LocalizationManager.GetText(itemData.TextKey + ".Name");
            imgItem.sprite = Resources.Load<Sprite>("Item/" + itemData.ID);
//            description.text = itemData.FlavorText;
            haveNum.text = num.ToString();
//            if(itemData.Type == 1) {
//                gcrm.GearRepository.Get(itemData.ID).Do(gearData => {
//                    SetGearData(gearData);
//                    gameObject.SetActive(true);
//                }).Subscribe();
//            } else {
                SetGearData();
                gameObject.SetActive(true);
//            }
        }
    }
}
