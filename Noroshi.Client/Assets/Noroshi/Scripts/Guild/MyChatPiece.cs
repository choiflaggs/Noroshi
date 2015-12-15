using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class MyChatPiece : MonoBehaviour {
        [SerializeField] Text txtPlayerName;
        [SerializeField] Text txtTime;
        [SerializeField] Text txtDummyComment;
        [SerializeField] Text txtComment;
        [SerializeField] Image imgPlayer;
        [SerializeField] LayoutElement layoutElement;
        [SerializeField] ContentSizeFitter contentSizeFitter;

        public void SetComment(Noroshi.Core.WebApi.Response.Guild.GuildChatMessage chatData, int width) {
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
            TweenNull.Add(gameObject, 0.01f).Then(() => {
                contentSizeFitter.enabled = false;
                if(txtDummyComment.GetComponent<RectTransform>().sizeDelta.x > width) {
                    txtDummyComment.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        width, txtDummyComment.GetComponent<RectTransform>().sizeDelta.y
                    );
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
                contentSizeFitter.enabled = true;
                TweenNull.Add(gameObject, 0.01f).Then(() => {
                    layoutElement.minWidth = width;
                    layoutElement.minHeight = txtDummyComment.GetComponent<RectTransform>().sizeDelta.y;
                    TweenNull.Add(gameObject, 0.01f).Then(() => {
                        txtDummyComment.transform.localPosition = new Vector2(
                            -width / 2, txtDummyComment.transform.localPosition.y
                        );
                    });
                });
            });
        }
    }
}
