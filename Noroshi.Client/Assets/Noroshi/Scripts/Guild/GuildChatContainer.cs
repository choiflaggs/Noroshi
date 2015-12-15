using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace Noroshi.UI {
    public class GuildChatContainer : MonoBehaviour {
        [SerializeField] Text txtGuildName;
        [SerializeField] Text txtCurrentMemberNum;
        [SerializeField] Text txtMaxMemberNum;
        [SerializeField] ScrollRect chatScroller;
        [SerializeField] GameObject chatWrapper;
        [SerializeField] BtnCommon btnPost;
        [SerializeField] InputField inputField;
        [SerializeField] ChatPiece chatPiecePref;
        [SerializeField] MyChatPiece myChatPiecePref;
        [SerializeField] int chatPieceWidth;
        [SerializeField] GameObject processing;

        private Noroshi.GuildChat.WebApiRequester webApiRequester = new Noroshi.GuildChat.WebApiRequester();
        private bool isBeginner;
        private uint currentMessageID;
        private uint currentCreatedAt;

        private void ViewComment(Noroshi.Core.WebApi.Response.Guild.GuildChatMessage[] messageList) {
            for(int i = messageList.Length - 1; i > -1; i--) {
                if(messageList[i].OtherPlayerStatus.ID == PlayerInfo.Instance.GetPlayerStatus().PlayerID) {
                    var piece = Instantiate(myChatPiecePref);
                    piece.transform.SetParent(chatWrapper.transform);
                    piece.transform.localScale = new Vector2(-1, 1);
                    piece.SetComment(messageList[i], chatPieceWidth);
                } else {
                    var piece = Instantiate(chatPiecePref);
                    piece.transform.SetParent(chatWrapper.transform);
                    piece.transform.localScale = Vector3.one;
                    piece.SetComment(messageList[i]);
                }
                if(i == 0) {
                    currentMessageID = messageList[i].ID;
                    currentCreatedAt = messageList[i].CreatedAt;
                }
            }
            TweenNull.Add(gameObject, 0.05f).Then(() => {
                chatScroller.verticalNormalizedPosition = 0;
            });
        }

        private void PostComment() {
            var text = inputField.text;
            if(text == "") {return;}
            processing.SetActive(true);
            btnPost.SetEnable(false);
            if(isBeginner) {
                webApiRequester.CreateBeginnerGuildMessage(text).Do(res => {
                    TweenNull.Add(gameObject, 0.8f).Then(() => {
                        webApiRequester.GetNewBeginnerGuildMessage(currentMessageID, currentCreatedAt).Do(data => {
                            ViewComment(data.Messages);
                            inputField .text = "";
                            btnPost.SetEnable(true);
                            processing.SetActive(false);
                        }).Subscribe();
                    });
                }).Subscribe();
            } else {
                webApiRequester.CreateNormalGuildMessage(text).Do(res => {
                    TweenNull.Add(gameObject, 0.8f).Then(() => {
                        webApiRequester.GetNewNormalGuildMessage(currentMessageID, currentCreatedAt).Do(data => {
                            ViewComment(data.Messages);
                            inputField .text = "";
                            btnPost.SetEnable(true);
                            processing.SetActive(false);
                        }).Subscribe();
                    });
                }).Subscribe();
            }
        }

        public void Init(Noroshi.Core.WebApi.Response.Guild.Guild guildData, bool flag) {
            isBeginner = flag;
            if(isBeginner) {
                webApiRequester.GetBeginnerGuildMessage().Do(data => {
                    ViewComment(data.Messages);
                }).Subscribe();
            } else {
                webApiRequester.GetNormalGuildMessage().Do(data => {
                    ViewComment(data.Messages);
                }).Subscribe();
            }

            btnPost.OnClickedBtn.Subscribe(_ => {
                PostComment();
            });

            txtGuildName.text = guildData.Name;
            txtCurrentMemberNum.text = guildData.MemberNum.ToString();
            txtMaxMemberNum.text = guildData.MaxMemberNum.ToString();
        }
    }
}
