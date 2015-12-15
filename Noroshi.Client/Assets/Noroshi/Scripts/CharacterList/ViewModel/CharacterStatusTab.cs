using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.CharacterList {
    public class CharacterStatusTab : MonoBehaviour {
        [SerializeField] GameObject strengthGrowthWrapper;
        [SerializeField] GameObject intellectGrowthWrapper;
        [SerializeField] GameObject agilityGrowthWrapper;
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
        [SerializeField] GameObject reduceEnergyCostWrapper;
        [SerializeField] GameObject improveHealingsWrapper;
        [SerializeField] Text txtStrengthGrowth;
        [SerializeField] Text txtIntellectGrowth;
        [SerializeField] Text txtAgilityGrowth;
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
        [SerializeField] Text txtReduceEnergyCost;
        [SerializeField] Text txtImproveHealings;

        public void SetData(CharacterPanel.CharaData data) {
            if(data.status.StrengthGrowth != 0) {
                txtStrengthGrowth.text = data.status.StrengthGrowth.ToString();
                strengthWrapper.SetActive(true);
            } else {strengthGrowthWrapper.SetActive(false);}
            if(data.status.IntellectGrowth != 0) {
                txtIntellectGrowth.text = data.status.IntellectGrowth.ToString();
                intellectGrowthWrapper.SetActive(true);
            } else {intellectGrowthWrapper.SetActive(false);}
            if(data.status.AgilityGrowth != 0) {
                txtAgilityGrowth.text = data.status.AgilityGrowth.ToString();
                agilityGrowthWrapper.SetActive(true);
            } else {agilityGrowthWrapper.SetActive(false);}
            if(data.status.Strength != 0) {
                txtStrength.text = data.status.Strength.ToString();
                strengthWrapper.SetActive(true);
            } else {strengthWrapper.SetActive(false);}
            if(data.status.Intellect != 0) {
                txtIntellect.text = data.status.Intellect.ToString();
                intellectWrapper.SetActive(true);
            } else {intellectWrapper.SetActive(false);}
            if(data.status.Agility != 0) {
                txtAgility.text = data.status.Agility.ToString();
                agilityWrapper.SetActive(true);
            } else {agilityWrapper.SetActive(false);}
            if(data.status.MaxHP != 0) {
                txtHp.text = data.status.MaxHP.ToString();
                hpWrapper.SetActive(true);
            } else {hpWrapper.SetActive(false);}
            if(data.status.PhysicalAttack != 0) {
                txtPhysicalAttack.text = data.status.PhysicalAttack.ToString();
                physicalAttackWrapper.SetActive(true);
            } else {physicalAttackWrapper.SetActive(false);}
            if(data.status.MagicPower != 0) {
                txtMagicPower.text = data.status.MagicPower.ToString();
                magicPowerWrapper.SetActive(true);
            } else {magicPowerWrapper.SetActive(false);}
            if(data.status.Armor != 0) {
                txtArmor.text = data.status.Armor.ToString();
                armorWrapper.SetActive(true);
            } else {armorWrapper.SetActive(false);}
            if(data.status.MagicRegistance != 0) {
                txtMagicResistance.text = data.status.MagicRegistance.ToString();
                magicResistanceWrapper.SetActive(true);
            } else {magicResistanceWrapper.SetActive(false);}
            if(data.status.PhysicalCrit != 0) {
                txtPhysicalCrit.text = data.status.PhysicalCrit.ToString();
                physicalCritWrapper.SetActive(true);
            } else {physicalCritWrapper.SetActive(false);}
            if(data.status.MagicCrit != 0) {
                txtMagicCrit.text = data.status.MagicCrit.ToString();
                magicCritWrapper.SetActive(true);
            } else {magicCritWrapper.SetActive(false);}
            if(data.status.HPRegen != 0) {
                txtHPRegen.text = data.status.HPRegen.ToString();
                hpRegenWrapper.SetActive(true);
            } else {hpRegenWrapper.SetActive(false);}
            if(data.status.EnergyRegen != 0) {
                txtEnergyRegen.text = data.status.EnergyRegen.ToString();
                energyRegenWrapper.SetActive(true);
            } else {energyRegenWrapper.SetActive(false);}
            if(data.status.Accuracy != 0) {
                txtAccuracy.text = data.status.Accuracy.ToString();
                accuracyWrapper.SetActive(true);
            } else {accuracyWrapper.SetActive(false);}
            if(data.status.Dodge != 0) {
                txtDodge.text = data.status.Dodge.ToString();
                dodgeWrapper.SetActive(true);
            } else {dodgeWrapper.SetActive(false);}
            if(data.status.ArmorPenetration != 0) {
                txtArmorPenetration.text = data.status.ArmorPenetration.ToString();
                armorPenetrationWrapper.SetActive(true);
            } else {armorPenetrationWrapper.SetActive(false);}
            if(data.status.IgnoreMagicResistance != 0) {
                txtIgnoreMagicResistance.text = data.status.IgnoreMagicResistance.ToString();
                ignoreMagicResistanceWrapper.SetActive(true);
            } else {ignoreMagicResistanceWrapper.SetActive(false);}
            if(data.status.LifeStealRating != 0) {
                txtLifeStealRating.text = data.status.LifeStealRating.ToString();
                lifeStealRatingWrapper.SetActive(true);
            } else {lifeStealRatingWrapper.SetActive(false);}
            if(data.status.ReduceEnergyCost != 0) {
                txtReduceEnergyCost.text = data.status.ReduceEnergyCost.ToString();
                reduceEnergyCostWrapper.SetActive(true);
            } else {reduceEnergyCostWrapper.SetActive(false);}
            if(data.status.ImproveHealings != 0) {
                txtImproveHealings.text = data.status.ImproveHealings.ToString();
                improveHealingsWrapper.SetActive(true);
            } else {improveHealingsWrapper.SetActive(false);}

        }
    }
}
