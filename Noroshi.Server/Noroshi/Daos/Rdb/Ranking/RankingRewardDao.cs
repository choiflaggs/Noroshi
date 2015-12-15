using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RankingRewardSchema;

namespace Noroshi.Server.Daos.Rdb.Ranking
{
    public class RankingRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByRankingID(uint rankingId)
        {
            return _select("RankingID = @RankingID", new { RankingID = rankingId });
        }
    }
}
