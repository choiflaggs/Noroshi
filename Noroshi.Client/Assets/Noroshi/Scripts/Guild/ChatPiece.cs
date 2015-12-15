using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class ChatPiece : MonoBehaviour {
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtTime;
        [SerializeField] Text txtDummyComment;
        [SerializeField] Text txtComment;
        [SerializeField] Image imgPlayer;

        public void SetComment(Noroshi.Core.WebApi.Response.Guild.GuildChatMessage chatData) {
            var t = Constant.UNIX_EPOCH.AddSeconds(chatData.CreatedAt).ToLocalTime();
            var hour = t.Hour < 10 ? "0" + t.Hour : t.Hour.ToString();
            var minute = t.Minute < 10 ? "0" + t.Minute : t.Minute.ToString();
            txtPlayerName.text = chatData.OtherPlayerStatus.Name;
            txtTime.text = t.Month + "/" + t.Day + " " + hour + ":" + minute;
            txtDummyComment.text = chatData.Message;
            txtComment.text = chatData.Message;
            imgPlayer.sprite = Resources.Load<Sprite>(
//                string.Format("Character/{0}/thumb_1", chatData.OtherPlayerStatus.AvaterCharacterID)
                string.Format("Character/{0}/thumb_1", 101)
            );
        }

    }
}
