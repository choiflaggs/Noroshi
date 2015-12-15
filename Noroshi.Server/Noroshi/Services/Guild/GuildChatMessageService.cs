using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Contexts;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Services.Guild
{
    public class GuildChatMessageService
    {

        public static GuildError CreateBeginnerGuildMessage(uint playerId, string message)
        {
            // 初心者ギルド[書き込み].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            // 初心者ギルド 書き込み権限チェック.
            if (!playerStatus.CanJoinBeginnerGuildChat())
            {
                return new GuildError() { NoAuthority = true };
            }
            var beginnerGuild = GuildEntity.ReadAndBuildBeginnerGuild(ReadType.Slave);

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                // 書込.
                var guildChatMessageEntity = GuildChatMessageEntity.Create(beginnerGuild.ID, playerStatus.PlayerID, message);
                if (guildChatMessageEntity == null)
                {
                    throw new SystemException(string.Join("\t", "Fail BeginnerGuildMessageWrite", playerId));
                }
                tx.Commit();
                return new GuildError() {};
            });
            
        }
        public static GuildChatMessageResponse GetBeginnerGuildMessage(uint playerId)
        {
            // 初心者ギルド[最新から30件取得].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            // 初心者ギルド 閲覧権限チェック.
            if (!playerStatus.CanJoinBeginnerGuildChat())
            {
                return new GuildChatMessageResponse() { Error = new GuildError() { NoAuthority = true} };
            }
            var beginnerGuild = GuildEntity.ReadAndBuildBeginnerGuild(ReadType.Slave);
            var guildChatMessage = GuildChatMessageEntity.ReadByGuildID(beginnerGuild.ID).ToArray();

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary( entity => entity.PlayerID );
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }


        public static GuildChatMessageResponse GetNewBeginnerGuildMessage(uint playerId, uint currentMessageId, uint currentCreatedAt)
        {
            // 初心者ギルド[リロード].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinBeginnerGuildChat())
            {
                return new GuildChatMessageResponse() {Error = new GuildError() {NoAuthority = true}};
            }

            var beginnerGuild = GuildEntity.ReadAndBuildBeginnerGuild(ReadType.Lock);
            var guildChatMessage =GuildChatMessageEntity.ReadNewByGuildIDAndCurrentCreatedAtAndCurrentID(beginnerGuild.ID, currentCreatedAt, currentMessageId).ToList();
            var rowCount = Constant.GUILD_CHAT_ROW_LIMIT - guildChatMessage.Count;
            if (rowCount > 0)
            {
                guildChatMessage.AddRange(GuildChatMessageEntity.ReadNewByGuildIDAndCurrentCreatedAt(beginnerGuild.ID, currentCreatedAt, (ushort) rowCount).ToList());
            }

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary(entity => entity.PlayerID);
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }

        public static GuildChatMessageResponse GetOldBeginnerGuildMessage(uint playerId, uint currentMessageId, uint currentCreatedAt)
        {
            // 初心者ギルド[過去ログ].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinBeginnerGuildChat())
            {
                return new GuildChatMessageResponse() { Error = new GuildError() { NoAuthority = true } };
            }

            var beginnerGuild = GuildEntity.ReadAndBuildBeginnerGuild(ReadType.Slave);
            var guildChatMessage = GuildChatMessageEntity.ReadOldByGuildIDAndCurrentCreatedAtAndCurrentID(beginnerGuild.ID, currentCreatedAt, currentMessageId).ToList();
            var rowCount = (ushort) (Constant.GUILD_CHAT_ROW_LIMIT - guildChatMessage.Count);
            if (0 < rowCount)
            {
                guildChatMessage.AddRange(GuildChatMessageEntity.ReadOldByGuildIDAndCurrentCreatedAt(beginnerGuild.ID, currentCreatedAt, rowCount).ToList());
            }

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary(entity => entity.PlayerID);
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }

        public static GuildError CreateNormalGuildMessage(uint playerId, string message)
        {
            // 通常ギルド [書き込み].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinNormaGuildChat((uint)playerStatus.GuildID))
            {
                return new GuildError() { NoAuthority = true };
            }

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                // 更新実行。
                var guildChatMessageEntity = GuildChatMessageEntity.Create((uint)playerStatus.GuildID, playerStatus.PlayerID, message);
                if (guildChatMessageEntity == null)
                {
                    throw new SystemException(string.Join("\t", "Fail NormalGuildMessageWrite", playerId));
                }
                tx.Commit();
                return new GuildError() { };
            });
        }

        public static GuildChatMessageResponse GetNormalGuildMessage(uint playerId)
        {
            // 通常ギルド[最新から30件取得].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinNormaGuildChat((uint)playerStatus.GuildID))
            {
                return new GuildChatMessageResponse() { Error = new GuildError() { NoAuthority = true } };
            }

            var guildChatMessage = GuildChatMessageEntity.ReadByGuildID((uint)playerStatus.GuildID).ToArray();

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary(entity => entity.PlayerID);
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }

        public static GuildChatMessageResponse GetNewNormalGuildMessage(uint playerId, uint currentMessageId, uint currentCreatedAt)
        {
            // 通常ギルド[リロード].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinNormaGuildChat((uint)playerStatus.GuildID))
            {
                return new GuildChatMessageResponse() { Error = new GuildError() { NoAuthority = true } };
            }

            var guildChatMessage = GuildChatMessageEntity.ReadNewByGuildIDAndCurrentCreatedAtAndCurrentID((uint)playerStatus.GuildID, currentCreatedAt, currentMessageId).ToList();
            var rowCount = Constant.GUILD_CHAT_ROW_LIMIT - guildChatMessage.Count;
            if (rowCount > 0)
            {
                guildChatMessage.AddRange(GuildChatMessageEntity.ReadNewByGuildIDAndCurrentCreatedAt((uint)playerStatus.GuildID, currentCreatedAt,(ushort)rowCount).ToList());
            }

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary(entity => entity.PlayerID);
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }

        public static GuildChatMessageResponse GetOldNormalGuildMessageMessage(uint playerId, uint currentMessageId, uint currentCreatedAt)
        {
            // 通常ギルド[過去ログ].

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Slave);

            if (!playerStatus.CanJoinNormaGuildChat((uint)playerStatus.GuildID))
            {
                return new GuildChatMessageResponse() { Error = new GuildError() { NoAuthority = true } };
            }

            var guildChatMessage = GuildChatMessageEntity.ReadOldByGuildIDAndCurrentCreatedAtAndCurrentID((uint)playerStatus.GuildID, currentCreatedAt, currentMessageId).ToList();
            var rowCount = Constant.GUILD_CHAT_ROW_LIMIT - guildChatMessage.Count;
            if (rowCount > 0)
            {
                guildChatMessage.AddRange(GuildChatMessageEntity.ReadOldByGuildIDAndCurrentCreatedAt((uint)playerStatus.GuildID, currentCreatedAt, (ushort)rowCount).ToList());
            }

            var submittedPlayerStatus = PlayerStatusEntity.ReadAndBuildMulti(guildChatMessage.Select(chat => chat.PlayerID).Distinct()).ToDictionary(entity => entity.PlayerID);
            return new GuildChatMessageResponse() { Messages = guildChatMessage.Select(d => { return SetContributorPlayerResponseData(d, submittedPlayerStatus); }).ToArray() };
        }



        private static GuildChatMessage SetContributorPlayerResponseData(GuildChatMessageEntity chat, Dictionary<uint, PlayerStatusEntity> contributor)
        {
            if ( chat.PlayerID == 0 || contributor.ContainsKey(chat.PlayerID) == false )
            {
                // システムメッセージの類と判断.
                return chat.ToResponseData(null);
            }

            // 投稿者の情報付与.
            return chat.ToResponseData(contributor[chat.PlayerID]);
        }
    }
}
