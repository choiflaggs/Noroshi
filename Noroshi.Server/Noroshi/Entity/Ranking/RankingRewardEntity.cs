using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Ranking;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RankingRewardSchema;

namespace Noroshi.Server.Entity.Ranking
{
    /// <summary>
    /// ランキング報酬クラス。
    /// </summary>
    public class RankingRewardEntity : AbstractDaoWrapperEntity<RankingRewardEntity, RankingRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// ランキングIDを指定してビルド。
        /// </summary>
        /// <param name="rankingId">対象ランキングID</param>
        /// <returns></returns>
        public static IEnumerable<RankingRewardEntity> ReadAndBuildByRankingID(uint rankingId)
        {
            return _instantiate((new RankingRewardDao()).ReadByRankingID(rankingId));
        }

        public uint RankingID => _record.RankingID;
        /// <summary>
        /// 順位敷居値。
        /// </summary>
        public uint ThresholdRank => _record.ThresholdRank;
        /// <summary>
        /// 番号（ユニーク制約、表示順のために利用しているだけ）。
        /// </summary>
        public byte No => _record.No;

        public PossessionParam GetPossessionParam()
        {
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.PossessionCategory,
                ID = _record.PossessionID,
                Num = _record.PossessionNum,
            };
        }
    }
}
