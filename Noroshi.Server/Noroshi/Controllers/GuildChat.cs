using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Server.Services.Guild;

namespace Noroshi.Server.Controllers
{
    public class GuildChat : AbstractController
    {

        [Post]
        public GuildError CreateBeginnerGuildMessage(string message)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.CreateBeginnerGuildMessage(playerId, message);
        }

        [Post]
        public GuildChatMessageResponse GetBeginnerGuildMessage()
        {
            // チャットを開いたタイミングで最新から30件を取得する.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetBeginnerGuildMessage(playerId);
        }

        [Post]
        public GuildChatMessageResponse GetNewBeginnerGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            // ギルドメンバーの書き込みがあった場合読み取り.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetNewBeginnerGuildMessage(playerId, currentMessageId, currentCreatedAt);
        }

        [Post]
        public GuildChatMessageResponse GetOldBeginnerGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            // ギルドメンバーの書き込みがあった場合読み取り.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetOldBeginnerGuildMessage(playerId, currentMessageId, currentCreatedAt);
        }

        [Post]
        public GuildError CreateNormalGuildMessage(string message)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.CreateNormalGuildMessage(playerId, message);
        }

        [Post]
        public GuildChatMessageResponse GetNormalGuildMessage()
        {
            // チャットを開いたタイミングで最新から30件を取得する.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetNormalGuildMessage(playerId);
        }

        [Post]
        public GuildChatMessageResponse GetNewNormalGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            // ギルドメンバーの書き込みがあった場合読み取り.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetNewNormalGuildMessage(playerId, currentMessageId, currentCreatedAt);
        }

        [Post]
        public GuildChatMessageResponse GetOldNormalGuildMessage(uint currentMessageId, uint currentCreatedAt)
        {
            // ギルドメンバーの書き込みがあった場合読み取り.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GuildChatMessageService.GetOldNormalGuildMessageMessage(playerId, currentMessageId, currentCreatedAt);

        }

    }
}