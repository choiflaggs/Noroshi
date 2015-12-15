using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class BossListPanel : MonoBehaviour {
        [SerializeField] Image imgBoss;
        [SerializeField] Text txtBossName;
        [SerializeField] Text txtBossLevel;
        [SerializeField] Text txtHP;
        [SerializeField] Text txtMaxHP;
        [SerializeField] Text txtRemainTime;
        [SerializeField] GameObject hpBar;
        [SerializeField] GameObject bigBossTag;
        [SerializeField] GameObject notAttackTag;
        [SerializeField] GameObject comboTag;
        [SerializeField] Text txtComboCount;
        [SerializeField] GameObject defeatOverlay;
        [SerializeField] GameObject timeupOverlay;

        public void SetPanel(Noroshi.Core.WebApi.Response.RaidBoss.RaidBoss bossData) {
            var timespan = (DateTime.UtcNow.ToUniversalTime() - Constant.UNIX_EPOCH).TotalSeconds;
            var hpBarWidth = hpBar.GetComponent<RectTransform>().sizeDelta.x;
            var xx = hpBarWidth - hpBarWidth * (float)bossData.CurrentHP / (float)bossData.MaxHP;
            if((float)bossData.CurrentHP / (float)bossData.MaxHP < 0.25f) {
                txtHP.color = Constant.BAR_COLOR_ALERT;
                hpBar.GetComponent<Image>().color = Constant.BAR_COLOR_ALERT;
            } else {
                txtHP.color = Constant.TEXT_COLOR_POSITIVE;
                hpBar.GetComponent<Image>().color = Constant.BAR_COLOR_NORMAL;
            }
            hpBar.transform.localPosition = new Vector3(-xx, 0, 0);
            txtBossName.text = bossData.TextKey;
            txtBossLevel.text = bossData.Level.ToString();
            txtHP.text = bossData.CurrentHP.ToString();
            txtMaxHP.text = bossData.MaxHP.ToString();
            if(bossData.OwnPlayerDamage == null) {
                notAttackTag.SetActive(true);
            } else if(bossData.ComboNum > 0) {
                txtComboCount.text = bossData.ComboNum.ToString();
                comboTag.SetActive(true);
            }
            if(bossData.IsDefeated) {
                txtRemainTime.text = "0";
                defeatOverlay.SetActive(true);
            } else if(bossData.EscapedAt - timespan <= 0) {
                txtRemainTime.text = "0";
                timeupOverlay.SetActive(true);
            } else {
                txtRemainTime.text = ((int)(bossData.EscapedAt - timespan) / 60).ToString();
            }
        }
    }
}
