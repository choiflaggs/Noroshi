using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class ArenaGuestContainer : MonoBehaviour {
        [SerializeField] Text txtName;
        [SerializeField] Text txtRank;
        [SerializeField] Text txtPower;
        [SerializeField] Text txtLevel;
        [SerializeField] Image imgCharacter;
        [SerializeField] BtnCommon btnBattle;

        public Subject<int> OnBattle = new Subject<int>();

        private void Start() {
            btnBattle.OnClickedBtn.Subscribe(id => {
                OnBattle.OnNext(id);
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
        }

        public void SetGuestInfo(Noroshi.Core.WebApi.Response.Players.PlayerArenaOtherResponse otherPlayer) {
            if(otherPlayer != null) {
//              var avaterID = otherPlayer.OtherPlayerStatus.AvaterCharacterID;
                var avaterID = 101;
                btnBattle.id = (int)otherPlayer.OtherPlayerStatus.ID;
                txtName.text = otherPlayer.OtherPlayerStatus.Name.ToString();
                txtRank.text = otherPlayer.Rank.ToString();
                txtPower.text = otherPlayer.AllStrength.ToString();
                txtLevel.text = otherPlayer.OtherPlayerStatus.Level.ToString();
                imgCharacter.sprite = Resources.Load<Sprite>(string.Format("Character/{0}/thumb_1", avaterID));
            } else {
                gameObject.SetActive(false);
            }
        }
    }
}
