using UniRx;
using Noroshi.Core.WebApi.Request.Guild;
using Noroshi.Core.WebApi.Response.Guild;

namespace Noroshi.GuildChat
{
    public class WebApiRequester
    {
    protected string _url()
    {
        return "GuildChat/";
    }

    static WebApi.WebApiRequester _getWebApiRequester()
    {
        return new WebApi.WebApiRequester();
    }

        /// <summary>
        /// ビギナーギルドへチャットメッセージを投稿する.
        /// </summary>
        public IObservable<GuildError> CreateBeginnerGuildMessage(string message)
        {
            var requestParams = new CreateGuildChatMessageRequest()
            {
                Message     = message
            };
            return _getWebApiRequester().Post<CreateGuildChatMessageRequest, GuildError>(_url() + "CreateBeginnerGuildMessage", requestParams);
        }

        /// <summary>
        /// ビギナーギルドのチャットから新着30件取得.
        /// </summary>
        public IObservable<GuildChatMessageResponse> GetBeginnerGuildMessage()
        {
            return _getWebApiRequester().Post<GuildChatMessageResponse>(_url() + "GetBeginnerGuildMessage");
        }

        /// <summary>
        /// ビギナーギルドチャットの最新投稿を現在のメッセージに継ぎ足し取得する(最大30件).
        /// </summary>
        public IObservable<GuildChatMessageResponse> GetNewBeginnerGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            var requestParams = new ReplenishCurrentGuildChatMessageRequest()
            {
                CurrentMessageId = currentMessageId,
                CurrentCreatedAt = currentCreatedAt,
            };
            return _getWebApiRequester().Post<ReplenishCurrentGuildChatMessageRequest,GuildChatMessageResponse>(_url() + "GetNewBeginnerGuildMessage",requestParams);
        }

        /// <summary>
        /// ビギナーギルドチャットの履歴を現在のメッセージに継ぎ足し取得する(最大30件).
        /// </summary>
        public IObservable<GuildChatMessageResponse> GetOldBeginnerGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            var requestParams = new ReplenishCurrentGuildChatMessageRequest()
            {
                CurrentMessageId = currentMessageId,
                CurrentCreatedAt = currentCreatedAt,
            };
            return _getWebApiRequester().Post<ReplenishCurrentGuildChatMessageRequest,GuildChatMessageResponse>(_url() + "GetOldBeginnerGuildMessage",requestParams);
        }

        /// <summary>
        /// ギルドへチャットメッセージを投稿する.
        /// </summary>
        public IObservable<GuildError> CreateNormalGuildMessage(string message)
        {
            var requestParams = new CreateGuildChatMessageRequest()
            {
                Message = message
            };
            return _getWebApiRequester().Post<CreateGuildChatMessageRequest, GuildError>(_url() + "CreateNormalGuildMessage", requestParams);
        }

        /// <summary>
        /// ギルドのチャットから新着30件取得.
        /// </summary>
        public IObservable<GuildChatMessageResponse> GetNormalGuildMessage()
        {
            return _getWebApiRequester().Post<GuildChatMessageResponse>(_url() + "GetNormalGuildMessage");
        }

        /// <summary>
        /// ギルドチャットの最新投稿を現在のメッセージに継ぎ足し取得する(最大30件).
        /// </summary>
        public IObservable<GuildChatMessageResponse> GetNewNormalGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            var requestParams = new ReplenishCurrentGuildChatMessageRequest()
            {
                CurrentMessageId = currentMessageId,
                CurrentCreatedAt = currentCreatedAt,
            };
            return _getWebApiRequester().Post<ReplenishCurrentGuildChatMessageRequest,GuildChatMessageResponse>(_url() + "GetNewNormalGuildMessage",requestParams);
        }

        /// <summary>
        /// ギルドチャットの履歴を現在のメッセージに継ぎ足し取得する(最大30件).
        /// </summary>
        public IObservable <GuildChatMessageResponse> GetOldNormalGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            var requestParams = new ReplenishCurrentGuildChatMessageRequest()
            {
                CurrentMessageId = currentMessageId,
                CurrentCreatedAt = currentCreatedAt,
            };
            return _getWebApiRequester().Post<ReplenishCurrentGuildChatMessageRequest,GuildChatMessageResponse>(_url() + "GetOldNormalGuildMessage",requestParams);
        }

    }
}
