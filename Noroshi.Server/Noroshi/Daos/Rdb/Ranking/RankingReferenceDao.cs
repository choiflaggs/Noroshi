using Schema = Noroshi.Server.Daos.Rdb.Schemas.RankingReferenceSchema;

namespace Noroshi.Server.Daos.Rdb.Ranking
{
    public class RankingReferenceDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        const byte DEFAULT_REFERENCE_ID = 1;

        protected override string _tableName => Schema.TableName;


        public Schema.Record ReadOrDefault(uint rankingId, ReadType readType = ReadType.Slave)
        {
            return ReadByPK(new Schema.PrimaryKey { RankingID = rankingId }, readType) ?? _getDefaultRecord(rankingId);
        }

        public Schema.Record CreateOrRead(uint rankingId)
        {
            return Create(_getDefaultRecord(rankingId)) ?? ReadByPK(new Schema.PrimaryKey { RankingID = rankingId }, ReadType.Lock);
        }

        public Schema.Record _getDefaultRecord(uint rankingId)
        {
            return new Schema.Record
            {
                RankingID = rankingId,
                ReferenceID = DEFAULT_REFERENCE_ID,
            };
        }
    }
}
