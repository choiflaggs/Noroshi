using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class RankingPlayerPiece : MonoBehaviour {
        [SerializeField] Text txtRanking;
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtScore;
        [SerializeField] Image imgChara;

        public void SetInfo(Noroshi.Core.WebApi.Response.RaidBoss.PlayerGuildRaidBoss data, int index) {
            if(index == 0) {
                txtRanking.text = (index + 1) + "<size=36>st</size>";
            } else if(index == 1) {
                txtRanking.text = (index + 1) + "<size=36>nd</size>";
            } else if(index == 2) {
                txtRanking.text = (index + 1) + "<size=36>rd</size>";
            } else {
                txtRanking.text = (index + 1) + "<size=36>th</size>";
            }
            txtPlayerName.text = data.Player.Name;
            txtScore.text = string.Format("{0:#,0}\r", data.Damage);
        }
    }
}
