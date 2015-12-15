using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Ranking;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRankingSchema;

namespace Noroshi.Server.Entity.Ranking
{
    /// <summary>
    /// ギルドランキングを扱うクラス。
    /// </summary>
    public class GuildRankingEntity : AbstractDaoWrapperEntity<GuildRankingEntity, GuildRankingDao, Schema.PrimaryKey, Schema.Record>
    {
        public static GuildRankingEntity ReadAndBuildByReferenceIDAndGuildID(uint rankingId, byte referenceId, uint guildId)
        {
            return ReadAndBuildByReferenceIDAndGuildIDs(rankingId, referenceId, new uint[] { guildId }).FirstOrDefault();
        }
        public static IEnumerable<GuildRankingEntity> ReadAndBuildByReferenceIDAndGuildIDs(uint rankingId, byte referenceId, IEnumerable<uint> guildIds)
        {
            return _instantiate((new GuildRankingDao(rankingId, referenceId)).ReadMultiByPKs(guildIds.Select(guildId => new Schema.PrimaryKey { GuildID = guildId })));
        }

        public static GuildRankingEntity ReadAndBuildByGuildID(uint rankingId, uint guildId)
        {
            return ReadAndBuildByGuildIDs(rankingId, new uint[] { guildId }).FirstOrDefault();
        }
        public static IEnumerable<GuildRankingEntity> ReadAndBuildByGuildIDs(uint rankingId, IEnumerable<uint> guildIds)
        {
            var reference = RankingReferenceEntity.ReadOrDefaultAndBuild(rankingId);
            return ReadAndBuildByReferenceIDAndGuildIDs(rankingId, reference.ReferenceID, guildIds);
        }

        public static bool CreateMulti(uint rankingId, byte referenceId, IEnumerable<GuildRankingDataEntity> datas)
        {
            return (new GuildRankingDao(rankingId, referenceId)).CreateMulti(referenceId, datas.Select(r => new Schema.Record
            {
                GuildID = r.GuildID,
                UniqueRank = r.UniqueRank,
                Rank = r.Rank,
                Value = r.Value,
            }));
        }

        /// <summary>
        /// TRUNCATE 文を発行。利用する際には注意！
        /// </summary>
        /// <param name="referenceId"></param>
        public static void Truncate(uint rankingId, byte referenceId)
        {
            (new GuildRankingDao(rankingId, referenceId)).Truncate();
        }
    }
}
