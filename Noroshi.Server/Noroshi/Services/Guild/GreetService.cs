using System;
using System.Linq;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Quest;

namespace Noroshi.Server.Services.Guild
{
    /// <summary>
    /// 挨拶サービス。
    /// </summary>
    public class GreetService : AbstractGuildService
    {
        /// <summary>
        /// 挨拶実行。
        /// </summary>
        /// <param name="playerId">挨拶する側プレイヤーID</param>
        /// <param name="targetPlayerId">挨拶される側プレイヤーID</param>
        /// <returns></returns>
        public static GreetResponse Greet(uint playerId, uint targetPlayerId)
        {
            // SLAVE チェック。
            var response = _validateWhenGreet(playerId, targetPlayerId, ReadType.Slave, (playerStatus, targetPlayerStatus, guild, playerRelation, possessionManager, error) =>
            {
                return new GreetResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenGreet(playerId, targetPlayerId, ReadType.Lock, (playerStatus, targetPlayerStatus, guild, playerRelation, possessionManager, error) =>
                {
                    if (error != null) return new GreetResponse { Error = error };

                    // 挨拶。
                    playerRelation.Greet(playerStatus);
                    targetPlayerStatus.IncrementUnconfirmedGreetedNum();
                    if (!playerRelation.Save() || !playerStatus.Save() || !targetPlayerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Update", playerStatus.PlayerID, targetPlayerStatus.PlayerID));
                    }
                    // 報酬付与
                    possessionManager.Add(playerRelation.GetGreetingRewards());

                    // ミッション:挨拶回数.
                    QuestTriggerEntity.CountUpGreetingNum(playerStatus.PlayerID);

                    tx.Commit();

                    return new GreetResponse
                    {
                        CurrentGreetingNum = playerStatus.GreetingNum,
                        MaxGreetingNum = guild.GetMaxGreetingNum(playerStatus.VipLevel),
                        GreetingRewards = possessionManager.GetPossessionObjects(playerRelation.GetGreetingRewards()).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }
        protected static T _validateWhenGreet<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, PlayerRelationEntity, PossessionManager, GuildError, T> func)
        {
            return _validateWithOtherPlayerInSameGuild(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                if (error != null) return func(playerStatus, targetPlayerStatus, guild, null, null, error);

                // プレイヤー関係をビルド。
                var playerRelation = readType == ReadType.Lock ? PlayerRelationEntity.CreateOrReadAndBuild(playerId, targetPlayerId) : PlayerRelationEntity.ReadOrDefaultAndBuild(playerId, targetPlayerId);

                // 相手の最終あいさつ受け取り日時を取得。
                // 該当値は単純増加なので常に SLAVE チェックで支障ない。
                var targetLastConfirmedAt = PlayerConfirmationEntity.ReadLastGreetedConfirmedAt(targetPlayerStatus.PlayerID);

                // 挨拶可否チェック。
                if (!playerRelation.CanGreet(playerStatus, targetPlayerStatus, guild, targetLastConfirmedAt))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Greet", playerStatus.PlayerID, targetPlayerStatus.PlayerID));
                }
                // PossessionManager インスタンス化。
                var possessionManager = new PossessionManager(playerId, playerRelation.GetGreetingRewards());

                return func(playerStatus, targetPlayerStatus, guild, playerRelation, possessionManager, null);
            });
        }

        /// <summary>
        /// 被挨拶報酬を受け取る。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static ReceiveGreetedRewardResponse ReceiveGreetedReward(uint playerId)
        {
            // SLAVE チェック。
            var response = _validateWhenReceiveGreetedReward(playerId, ReadType.Slave, (playerStatus, guild, error) =>
            {
                return new ReceiveGreetedRewardResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenReceiveGreetedReward(playerId, ReadType.Lock, (playerStatus, guild, error) =>
                {
                    if (error != null) return new ReceiveGreetedRewardResponse { Error = error };

                    // 獲得できる報酬。
                    var rewards = playerStatus.GetReceivableGreetedRewardPossessionParams();

                    // 未確認被挨拶数リセット
                    playerStatus.ResetUnconfirmedGreetedNum();
                    if (!playerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Update", playerStatus.PlayerID));
                    }
                    // 受け取り日時更新。
                    PlayerConfirmationEntity.ConfirmGreeted(playerStatus.PlayerID, ContextContainer.GetContext().TimeHandler.UnixTime);

                    // Possession 付与。
                    var possessionManager = new PossessionManager(playerId, rewards);
                    possessionManager.Add(rewards);

                    tx.Commit();

                    return new ReceiveGreetedRewardResponse
                    {
                        GreetedRewards = possessionManager.GetPossessionObjects(playerStatus.GetReceivableGreetedRewardPossessionParams()).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }
        static T _validateWhenReceiveGreetedReward<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateInGuild(playerId, readType, (playerStatus, guild, error) =>
            {
                if (error != null) func(playerStatus, guild, error);

                // 受け取り可否チェック。
                if (!playerStatus.CanReceiveGreetedReward())
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Receive Greeted Reward", playerStatus.PlayerID));
                }

                return func(playerStatus, guild, null);
            });
        }
    }
}
