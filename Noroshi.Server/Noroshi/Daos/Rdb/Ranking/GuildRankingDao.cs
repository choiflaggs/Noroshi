using System.Collections.Generic;
using System.Linq;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRankingSchema;

namespace Noroshi.Server.Daos.Rdb.Ranking
{
    public class GuildRankingDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        static readonly Dictionary<byte, string> SUFFIX_MAP = new Dictionary<byte, string>
        {
            { 1, "a" },
            { 2, "b" },
        };

        uint? _rankingId = null;
        byte? _referenceId = null;

        public GuildRankingDao() : base()
        {
        }
        public GuildRankingDao(uint rankingId, byte referenceId) : base()
        {
            _rankingId = rankingId;
            _referenceId = referenceId;
        }

        protected override string _tableName => $"guild_ranking_{_rankingId.Value}_{SUFFIX_MAP[_referenceId.Value]}";

        public bool CreateMulti(byte referenceId, IEnumerable<Schema.Record> records)
        {
            return CreateMulti(records).Count() == records.Count();
        }
    }
}
