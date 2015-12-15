using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Noroshi.Core.Game.Guild;
using RankingConstant = Noroshi.Core.Game.Ranking.Constant;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Ranking;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.PresentBox;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Guild
{
    public class GuildBatchService
    {
        const string RANKING_REWARD_PRESENT_BOX_TEXT_KEY_FORMAT = "Guild.Ranking.{0}.Reward";
        const string GUILD_RANK_REWARD_PRESENT_BOX_TEXT_KEY_FORMAT = "Guild.GuildRank.{0}.Reward";
        const ushort CREATE_RANKING_RECORD_NUM_PER_COMMIT = 2;
        const ushort CREATE_PRESENT_BOX_PLAYER_NUM_PER_COMMIT = 2;
        static readonly TimeSpan SLEEP_TIME_PER_COMMIT = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// ランキングを更新する。
        /// </summary>
        public static void UpdateRanking()
        {
            // ランキング種別が増えたら設定化する必要があるが、
            // 今はランキング ID 固定で、集計後に報酬付与。
            var rankingId = RankingConstant.DAILY_GUILD_RANKING_ID;
            _updateRankingAndGiveRewards(rankingId, GuildActivityDailyLogEntity.MakeYesterdayGuildDefeatRaidBossNumRankingDatas);
        }

        static void _updateRankingAndGiveRewards(uint rankingId, Func<List<GuildRankingDataEntity>> makeRankingDatas)
        {
            // 集計
            var rankingDatas = makeRankingDatas.Invoke();

            // ランキング更新
            _updateRanking(rankingId, rankingDatas);

            // 報酬付与
            _giveRewards(rankingId, rankingDatas);
        }

        static void _updateRanking(uint rankingId, List<GuildRankingDataEntity> rankingDatas)
        {
            // Master から参照テーブルをビルド。
            var rankingReference = RankingReferenceEntity.ReadOrDefaultAndBuild(rankingId, ReadType.Master);

            // これからレコードを作成していくランキングテーブルをトランケート。
            GuildRankingEntity.Truncate(rankingId, rankingReference.NextReferenceID);

            // レコード作成。
            _create(rankingId, rankingReference.NextReferenceID, rankingDatas);

            // 参照ランキングテーブルをスイッチ。
            rankingReference = RankingReferenceEntity.CreateOrReadAndBuild(rankingId);
            if (!rankingReference.Switch())
            {
                throw new SystemException(string.Join("\t", "Fail to Switch", rankingId));
            }
        }
        static void _create(uint rankingId, byte referenceId, List<GuildRankingDataEntity> rankingDatas)
        {
            foreach (var rankingDatasPerCommit in rankingDatas.Buffer(CREATE_RANKING_RECORD_NUM_PER_COMMIT))
            {
                _createPerTransaction(rankingId, referenceId, rankingDatasPerCommit);
                _clearRdbConnectionsAndSleep(SLEEP_TIME_PER_COMMIT);
            }
        }
        static void _createPerTransaction(uint rankingId, byte referenceId, GuildRankingDataEntity[] rankingDatas)
        {
            ContextContainer.ShardTransaction(tx =>
            {
                if (!GuildRankingEntity.CreateMulti(rankingId, referenceId, rankingDatas))
                {
                    throw new SystemException(string.Join("\t", "Fail to Create Multi", rankingId, referenceId));
                }
                tx.Commit();
            });
        }

        static void _giveRewards(uint rankingId, List<GuildRankingDataEntity> rankingDatas)
        {
            var allRewards = RankingRewardEntity.ReadAndBuildByRankingID(rankingId);
            foreach (var rankingData in rankingDatas)
            {
                var rewards = allRewards.Where(r => r.ThresholdRank >= rankingData.Rank).Select(r => r.GetPossessionParam());
                var guild = GuildEntity.ReadAndBuild(rankingData.GuildID);
                if (guild != null)
                {
                    var playerIds = guild.GetMemberPlayerStatuses().Select(ps => ps.PlayerID).ToList();
                    foreach (var playerIdsPerCommit in playerIds.Buffer(CREATE_PRESENT_BOX_PLAYER_NUM_PER_COMMIT))
                    {
                        PresentBoxEntity.AddRewardsAndCommit(playerIdsPerCommit, rewards, string.Format(RANKING_REWARD_PRESENT_BOX_TEXT_KEY_FORMAT, rankingId), new string[0], (playerId, reward) =>
                        {
                            ContextContainer.GetContext().Logger.Info("GuildRankingReward", rankingId, playerId, reward.Category, reward.ID, reward.Num);
                        });
                        _clearRdbConnectionsAndSleep(SLEEP_TIME_PER_COMMIT);
                    }
                }
            }
        }

        /// <summary>
        /// ギルドを最適化する。
        /// </summary>
        public static void OptimizeGuild()
        {
            // 一定期間経過した通常ギルドが対象。
            var guilds = GuildEntity.ReadAndBuildAllNormalGuilds()
                .Where(guild => guild.CreatedAt + (uint)Constant.NECESSARY_TIME_TO_OPTIMIZE.TotalSeconds < ContextContainer.GetContext().TimeHandler.UnixTime);
            foreach (var guild in guilds)
            {
                _optimize(guild);
                _clearRdbConnectionsAndSleep(SLEEP_TIME_PER_COMMIT);
            }
        }
        static void _optimize(GuildEntity guild)
        {
            // ギルドアクティビティ取得。
            var guildLogs = GuildActivityDailyLogEntity.ReadAndBuildThisWeekByGuildID(guild.ID);
            var guildWeeklyBPConsuming = guildLogs.Sum(log => log.BPConsuming);

            ContextContainer.ShardTransaction(tx =>
            {
                guild = GuildEntity.ReadAndBuild(guild.ID, ReadType.Lock);
                var members = guild.GetMemberPlayerStatuses(ReadType.Lock);

                // 解散した後でも利用できるようにログ用ギルドID確保。
                var guildId = guild.ID;

                IEnumerable<PlayerStatusEntity> layoffMembers;
                // ギルドアクティビティが足りない場合は全員を除名対象にする（結果的にギルド解散）。
                if (guildWeeklyBPConsuming < Constant.BREAK_GUILD_WEEKLY_BP_CONSUMING_THRESHOLD)
                {
                    layoffMembers = members;
                }
                // そうでない場合はギルドメンバーでアクティビティが低いプレイヤーを除名対象にする。
                else
                {
                    var playerLogs = PlayerActivityDailyLogEntity.ReadAndBuildThisWeekByPlayerIDs(members.Select(ps => ps.PlayerID));
                    var playerWeeklyStaminaConsumingMap = playerLogs
                        .ToLookup(log => log.PlayerID)
                        .ToDictionary(grouping => grouping.Key, grouping => grouping.Sum(log => log.StaminaConsuming));
                    layoffMembers = members.Where(ps => !playerWeeklyStaminaConsumingMap.ContainsKey(ps.PlayerID) || playerWeeklyStaminaConsumingMap[ps.PlayerID] < Constant.LAYOFF_PLAYER_WEEKLY_STAMINA_CONSUMING_THRESHOLD);
                }

                // 役職なし、幹部、リーダーの順で除名。
                foreach (var member in layoffMembers.OrderByDescending(ps => _getDropOutPriority(ps)))
                {
                    if (!guild.ForceDropOut(member, members) || !member.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Drop Out", member.PlayerID, guildId));
                    }
                    tx.AddAfterCommitAction(() =>
                    {
                        ContextContainer.GetContext().Logger.Info("GuildOptimization", member.PlayerID, guildId);
                    });
                }
                // 平均レベル更新
                if (guild.HasRecord)
                {
                    guild.SetAveragePlayerLevel((ushort)members.Except(layoffMembers).Average(ps => ps.Level));
                    if (!guild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Guild", guild.ID));
                    }
                }

                tx.Commit();
            });
        }
        // 除名優先度。
        static int _getDropOutPriority(PlayerStatusEntity playerStatus)
        {
            return
                playerStatus.GuildRole.HasValue && playerStatus.GuildRole.Value == GuildRole.Leader ? 0 :
                playerStatus.GuildRole.HasValue && playerStatus.GuildRole.Value == GuildRole.Executive ? 1 :
                2;
        }

        public static void GiveGuildRankReward()
        {
            var activities = GuildActivityDailyLogEntity.ReadAndBuildAllYesterday();
            var guildMap = GuildEntity.ReadAndBuildAllNormalGuilds().ToDictionary(guild => guild.ID);
            var guildRankToRewards = GuildRankRewardEntity.ReadAndBuildAll().ToLookup(grr => grr.GuildRank);
            foreach (var activity in activities)
            {
                var guildRank = (new CooperationPoint(activity.CooperationPoint)).GetGuildRank();
                var guild = guildMap.ContainsKey(activity.GuildID) ? guildMap[activity.GuildID] : null;
                var rewards = guildRankToRewards.Contains(guildRank) ? guildRankToRewards[guildRank].Select(grr => grr.GetPossessionParam()) : new PossessionParam[0];
                if (guild != null && rewards.Count() > 0)
                {
                    _giveGuildRankReward(guildRank, guild, rewards);
                    _clearRdbConnectionsAndSleep(SLEEP_TIME_PER_COMMIT);
                }
            }
        }
        static void _giveGuildRankReward(GuildRank guildRank, GuildEntity guild, IEnumerable<PossessionParam> rewards)
        {
            var playerIds = guild.GetMemberPlayerStatuses().Select(ps => ps.PlayerID).ToList();
            foreach (var playerIdsPerCommit in playerIds.Buffer(CREATE_PRESENT_BOX_PLAYER_NUM_PER_COMMIT))
            {
                PresentBoxEntity.AddRewardsAndCommit(playerIdsPerCommit, rewards, string.Format(GUILD_RANK_REWARD_PRESENT_BOX_TEXT_KEY_FORMAT, guildRank), new string[0], (playerId, reward) =>
                {
                    ContextContainer.GetContext().Logger.Info("GuildRankingReward", guildRank, playerId, reward.Category, reward.ID, reward.Num);
                });
                _clearRdbConnectionsAndSleep(SLEEP_TIME_PER_COMMIT);
            }
        }

        static void _clearRdbConnectionsAndSleep(TimeSpan time)
        {
            ContextContainer.GetContext().ClearRdbConnections();
            Thread.Sleep((int)time.TotalMilliseconds);
        }
    }
    /// <summary>
    /// 拡張メソッド用。
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Buffer<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count <= 0) throw new ArgumentOutOfRangeException("count");

            var buffer = new List<T>();
            foreach (var item in source)
            {
                buffer.Add(item);
                if (buffer.Count == count)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }
            if (buffer.Count > 0)
            {
                yield return buffer.ToArray();
            }
        }
    }
}
