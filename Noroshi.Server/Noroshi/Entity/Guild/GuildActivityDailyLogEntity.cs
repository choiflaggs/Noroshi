using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Ranking;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildActivityDailyLogSchema;

namespace Noroshi.Server.Entity.Guild
{
    public class GuildActivityDailyLogEntity : AbstractDaoWrapperEntity<GuildActivityDailyLogEntity, GuildActivityDailyLogDao, Schema.PrimaryKey, Schema.Record>
    {
        public static GuildActivityDailyLogEntity ReadAndBuildTodayByGuildID(uint guildId)
        {
            return ReadAndBuildTodayByGuildIDs(new uint[] { guildId }).FirstOrDefault();
        }
        public static IEnumerable<GuildActivityDailyLogEntity> ReadAndBuildTodayByGuildIDs(IEnumerable<uint> guildIds)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC;
            return ReadAndBuildMulti(guildIds.Select(guildId => new Schema.PrimaryKey { GuildID = guildId, CreatedOn = createdOn }));
        }

        public static GuildActivityDailyLogEntity CreateOrReadAndBuildTodayActivityLog(uint guildId)
        {
            return CreateOrReadAndBuildTodayActivityLogs(new[] { guildId }).FirstOrDefault();
        }
        public static IEnumerable<GuildActivityDailyLogEntity> CreateOrReadAndBuildTodayActivityLogs(IEnumerable<uint> guildIds)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC;
            return guildIds.Select(guildId => _instantiate((new GuildActivityDailyLogDao()).CreateOrRead(guildId, createdOn)));
        }

        public static IEnumerable<GuildActivityDailyLogEntity> ReadOrInstantiateDefaultAndBuildMultiYesterday(IEnumerable<uint> guildIds)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - 3600 * 24;
            var map = ReadAndBuildMulti(guildIds.Select(guildId => new Schema.PrimaryKey { GuildID = guildId, CreatedOn = createdOn })).ToDictionary(entity => entity.GuildID);
            return guildIds.Select(guildId => map.ContainsKey(guildId) ? map[guildId] : _instantiate((new GuildActivityDailyLogDao()).GetDefaultRecord(guildId, createdOn)));
        }
        public static IEnumerable<GuildActivityDailyLogEntity> ReadAndBuildAllYesterday()
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - 3600 * 24;
            return _instantiate((new GuildActivityDailyLogDao()).ReadByCreatedOn(createdOn));
        }
        public static IEnumerable<GuildActivityDailyLogEntity> ReadAndBuildThisWeekByGuildID(uint guildId)
        {
            var createdOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC - 3600 * 24 * 7;
            return _instantiate((new GuildActivityDailyLogDao()).ReadByGuildIDAndMinCreatedOn(guildId, createdOn));
        }

        /// <summary>
        /// 本日分の友情ポイントを加算する。
        /// </summary>
        /// <param name="guildId">対象ギルド ID</param>
        /// <param name="cooperationPoint">友情ポイント</param>
        public static void AddTodayConsumeCooperationPoint(uint guildId, ushort cooperationPoint)
        {
            var entity = CreateOrReadAndBuildTodayActivityLog(guildId);
            entity.AddConsumeCooperationPoint(cooperationPoint);
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Guild Activity Daily Log", guildId));
            }
        }
        /// <summary>
        /// 本日分のレイドボス撃破数をインクリメントする。
        /// </summary>
        /// <param name="guildId">対象ギルド ID</param>
        public static void IncrementTodayDefeatRaidBossNum(uint guildId)
        {
            var entity = CreateOrReadAndBuildTodayActivityLog(guildId);
            entity.IncrementDefeatRaidBossNum();
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Guild Activity Daily Log", guildId));
            }
        }

        /// <summary>
        /// 前日分のレイドボス撃破数ランキングデータを作成する。
        /// バッチ処理からしか呼んではいけない。
        /// </summary>
        /// <returns></returns>
        public static List<GuildRankingDataEntity> MakeYesterdayGuildDefeatRaidBossNumRankingDatas()
        {
            var logGroupByDefeatRaidBossNum = ReadAndBuildAllYesterday()
                .Where(log => log.DefeatRaidBossNum > 0)
                .GroupBy(log => log.DefeatRaidBossNum)
                .OrderByDescending(log => log.Key);

            uint rank = 1;
            uint uniqueRank = 1;
            var rankingDatas = new List<GuildRankingDataEntity>();
            foreach (var logs in logGroupByDefeatRaidBossNum)
            {
                rankingDatas.AddRange(logs.Select(log => new GuildRankingDataEntity(log.GuildID, uniqueRank++, rank, (int)logs.Key)));
                rank += (uint)logs.Count();
            }
            return rankingDatas;
        }


        public uint GuildID => _record.GuildID;
        public uint BPConsuming => _record.BPConsuming;
        public uint DefeatRaidBossNum => _record.DefeatRaidBossNum;
        public ushort CooperationPoint => _record.CooperationPoint;

        public void ConsumeBP(byte bp)
        {
            var record = _cloneRecord();
            record.BPConsuming += bp;
            _changeLocalRecord(record);
        }

        public void IncrementDefeatRaidBossNum()
        {
            var record = _cloneRecord();
            record.DefeatRaidBossNum++;
            _changeLocalRecord(record);
        }

        public void AddConsumeCooperationPoint(ushort cooperationPoint)
        {
            var record = _cloneRecord();
            record.CooperationPoint += cooperationPoint;
            _changeLocalRecord(record);
        }
    }
}
