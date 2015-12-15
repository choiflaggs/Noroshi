using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.RaidBoss;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.RaidBoss;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class RaidBossService
    {
        /// <summary>
        /// レイドボス一覧を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <returns></returns>
        public static ListResponse List(uint playerId)
        {
            return _validate(playerId, ReadType.Slave, (playerStatus, guild, error) =>
            {
                if (error != null) return new ListResponse { Error = error };

                // 出現中レイドボスをビルド。
                var activeGuildRaidBosses = GuildRaidBossEntity.ReadAndBuildAliveByGuildID(guild.ID).Where(grb => grb.IsActive());
                var activePlayerGuildRaidBosses = activeGuildRaidBosses.Count() > 0
                    ? PlayerGuildRaidBossEntity.ReadAndBuildByPlayerIDAndGuildRaidBosses(playerId, activeGuildRaidBosses)
                    : new PlayerGuildRaidBossEntity[0];

                // 報酬未受け取りレイドボスをビルド。
                var rewardUnreceivedPlayerGuildRaidBosses = PlayerGuildRaidBossEntity.ReadAndBuildBattleStateByPlayerIDOrderByGuildRaidBossCreatedAtAsc(playerId)
                    .Where(pgrb => activePlayerGuildRaidBosses.All(apgrb => apgrb.GuildRaidBossID != pgrb.GuildRaidBossID));
                var rewardUnreceivedGuildRaidBosses = rewardUnreceivedPlayerGuildRaidBosses.Count() > 0
                    ? GuildRaidBossEntity.ReadAndBuildMulti(rewardUnreceivedPlayerGuildRaidBosses.Select(pg => pg.GuildRaidBossID))
                    : new GuildRaidBossEntity[0];

                // 報酬ロード。
                var rewards = new List<PossessionParam>();
                rewards.AddRange(activeGuildRaidBosses.SelectMany(grb => grb.GetAllRewards()));
                rewards.AddRange(rewardUnreceivedGuildRaidBosses.SelectMany(grb => grb.GetAllRewards()));
                var possessionManager = new PossessionManager(playerId, rewards);
                possessionManager.Load();

                // レスポンスのためのマッピングデータ。
                var guildRaidBossIdToPlayerGuildRaidBossMap = activePlayerGuildRaidBosses
                    .Concat(rewardUnreceivedPlayerGuildRaidBosses)
                    .ToDictionary(pgrb => pgrb.GuildRaidBossID);

                return new ListResponse
                {
                    Guild = guild.ToResponseData(),
                    ActiveRaidBosses = activeGuildRaidBosses.Select(grb => grb.ToResponseData(guildRaidBossIdToPlayerGuildRaidBossMap.ContainsKey(grb.ID) ? guildRaidBossIdToPlayerGuildRaidBossMap[grb.ID] : null, possessionManager)).ToArray(),
                    RewardUnreceivedRaidBosses = rewardUnreceivedGuildRaidBosses.Select(grb => grb.ToResponseData(guildRaidBossIdToPlayerGuildRaidBossMap.ContainsKey(grb.ID) ? guildRaidBossIdToPlayerGuildRaidBossMap[grb.ID] : null, possessionManager)).ToArray(),
                };
            });
        }

        /// <summary>
        /// レイドボス詳細を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="guildRaidBossId">対象ギルドレイドボス ID</param>
        /// <returns></returns>
        public static ShowResponse Show(uint playerId, uint guildRaidBossId)
        {
            return _validateWithGuildRaidBoss(playerId, guildRaidBossId, ReadType.Slave, (playerStatus, guild, guildRaidBoss, possessionManager, error) =>
            {
                if (error != null) return new ShowResponse { Error = error };

                var playerGuildRaidBoss = PlayerGuildRaidBossEntity.ReadAndBuild(playerId, guildRaidBoss.ID, guildRaidBoss.CreatedAt);
                var ranking = PlayerGuildRaidBossEntity.ReadAndBuildByGuildRaidBossIDAndRaidBossCreatedAtOrderByDamageDesc(guildRaidBoss.ID, guildRaidBoss.CreatedAt, Constant.MAX_DAMAGE_RANKING_SHOWABLE_PLAYER_NUM);
                var logs = GuildRaidBossLogEntity.ReadAndBuildByGuildRaidBossIDOrderByCreatedAtDesc(guildRaidBoss.ID, guildRaidBoss.CreatedAt, Constant.MAX_SHOWABLE_GUILD_RAID_BOSS_LOG_NUM);

                // 関連するプレイヤー情報は一括でビルド。
                var playerIds = new List<uint>(ranking.Select(pg => pg.PlayerID));
                playerIds.AddRange(logs.Select(log => log.PlayerID));
                var playerStatusMap = playerIds.Count() == 0
                    ? new Dictionary<uint, PlayerStatusEntity>()
                    : PlayerStatusEntity.ReadAndBuildMulti(playerIds.Distinct()).ToDictionary(ps => ps.PlayerID);

                possessionManager.Load();

                return new ShowResponse
                {
                    RaidBoss = guildRaidBoss.ToResponseData(playerGuildRaidBoss, possessionManager),
                    Logs = logs.Select(log => log.ToResponseData(playerStatusMap[log.PlayerID])).ToArray(),
                    DamageRanking = ranking.Select(pg => pg.ToResponseData(playerStatusMap[pg.PlayerID])).ToArray(),
                };
            });
        }

        /// <summary>
        /// 報酬を受け取る。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="guildRaidBossId">対象ギルドレイドボス ID</param>
        /// <returns></returns>
        public static ReceiveRewardResponse ReceiveReward(uint playerId, uint guildRaidBossId)
        {
            // SLAVE チェック。
            var response = _validateWhenReceiveReward(playerId, guildRaidBossId, ReadType.Slave, (playerStatus, guild, guildRaidBoss, playerGuildRaidBoss, possessionManager, error) =>
            {
                return new ReceiveRewardResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenReceiveReward(playerId, guildRaidBossId, ReadType.Lock, (playerStatus, guild, guildRaidBoss, playerGuildRaidBoss, possessionManager, error) =>
                {
                    if (error != null) return new ReceiveRewardResponse { Error = error };

                    // 更新実行。
                    guildRaidBoss.ReceiveRewards(playerGuildRaidBoss);
                    if (!playerGuildRaidBoss.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Player Guild Raid Boss", playerStatus.PlayerID, guildRaidBoss.ID));
                    }
                    // 報酬を抽選。
                    var discoveryRewards = guildRaidBoss.LotAcquirableDiscoveryRewards(playerGuildRaidBoss, playerStatus.VipLevel);
                    var entryRewards = guildRaidBoss.LotAcquirableEntryRewards(playerGuildRaidBoss, playerStatus.VipLevel);

                    // 報酬付与。
                    var rewards = new List<PossessionParam>();
                    rewards.AddRange(discoveryRewards);
                    rewards.AddRange(entryRewards);
                    possessionManager.Add(rewards);

                    tx.Commit();

                    return new ReceiveRewardResponse
                    {
                        DiscoveryRewards = possessionManager.GetPossessionObjects(discoveryRewards).Select(po => po.ToResponseData()).ToArray(),
                        EntryRewards = possessionManager.GetPossessionObjects(entryRewards).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }
        static T _validateWhenReceiveReward<T>(uint playerId, uint guildRaidBossId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRaidBossEntity, PlayerGuildRaidBossEntity, PossessionManager, RaidBossError, T> func)
        {
            return _validateWithGuildRaidBoss(playerId, guildRaidBossId, readType, (playerStatus, guild, guildRaidBoss, possessionManager, error) =>
            {
                if (error != null) return func.Invoke(playerStatus, guild, guildRaidBoss, null, possessionManager, error);

                // 対象レイドボス状態チェック。
                var playerGuildRaidBoss = PlayerGuildRaidBossEntity.ReadAndBuild(playerStatus.PlayerID, guildRaidBoss.ID, guildRaidBoss.CreatedAt, readType);
                if (!guildRaidBoss.CanReceiveRewards(playerGuildRaidBoss, playerStatus.VipLevel))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Receive Reward", playerStatus.PlayerID, guildRaidBoss.ID));
                }
                return func.Invoke(playerStatus, guild, guildRaidBoss, playerGuildRaidBoss, possessionManager, error);
            });
        }

        static T _validateWithGuildRaidBoss<T>(uint playerId, uint guildRaidBossId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRaidBossEntity, PossessionManager, RaidBossError, T> func)
        {
            return _validate(playerId, readType, (playerStatus, guild, error) =>
            {
                if (error != null) return func.Invoke(playerStatus, guild, null, null, error);

                // 対象レイドボスをビルド。
                var guildRaidBoss = GuildRaidBossEntity.ReadAndBuild(guildRaidBossId, readType);
                // 対象レイドボスが存在しない、もしくは所属ギルドと無関係であれば、
                if (guildRaidBoss == null || guildRaidBoss.GuildID != guild.ID)
                {
                    // 見つかりませんエラー。
                    return func.Invoke(playerStatus, guild, null, null, new RaidBossError { RaidBossNotFound = true });
                }
                // PossessionManager をインスタンス化。
                var possessionManager = new PossessionManager(playerId, guildRaidBoss.GetAllRewards());

                return func.Invoke(playerStatus, guild, guildRaidBoss, possessionManager, null);
            });
        }
        static T _validate<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, RaidBossError, T> func)
        {
            // プレイヤーをビルド。
            var playerStatus = _buildOwnPlayerStatus(playerId, readType);

            // レイドボス関連処理はギルド所属必須。
            if (!playerStatus.GuildID.HasValue)
            {
                func.Invoke(playerStatus, null, new RaidBossError { GuildNotFound = true });
            }
            // 所属ギルドをビルド。
            var guild = GuildEntity.ReadAndBuild(playerStatus.GuildID.Value, readType);
            if (guild == null)
            {
                throw new SystemException(string.Join("\t", "Guild Not Found", playerStatus.PlayerID, playerStatus.GuildID.Value));
            }
            return func.Invoke(playerStatus, guild, null);
        }
        static PlayerStatusEntity _buildOwnPlayerStatus(uint playerId, ReadType readType)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, readType);
            if (playerStatus == null)
            {
                throw new SystemException(string.Join("\t", "Player Status Not Found", playerId));
            }
            return playerStatus;
        }
    }
}
