using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class BattleLogPiece : MonoBehaviour {
        [SerializeField] Text txtTime;
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtDamage;

        public void SetInfo(Noroshi.Core.WebApi.Response.RaidBoss.RaidBossLog data) {
            var t = Constant.UNIX_EPOCH.AddSeconds(data.CreatedAt).ToLocalTime();
            var hour = t.Hour < 10 ? "0" + t.Hour : t.Hour.ToString();
            var minute = t.Minute < 10 ? "0" + t.Minute : t.Minute.ToString();
            txtTime.text = hour + ":" + minute;
            txtPlayerName.text = data.Player.Name;
            txtDamage.text = string.Format("{0:#,0}\r", data.Damage);
        }
    }
}
