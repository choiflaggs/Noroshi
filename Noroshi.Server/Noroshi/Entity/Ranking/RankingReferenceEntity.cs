using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Ranking;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RankingReferenceSchema;

namespace Noroshi.Server.Entity.Ranking
{
    /// <summary>
    /// ランキングテーブル参照管理クラス。
    /// </summary>
    public class RankingReferenceEntity : AbstractDaoWrapperEntity<RankingReferenceEntity, RankingReferenceDao, Schema.PrimaryKey, Schema.Record>
    {
        const byte DEFAULT_REFERENCE_ID = 1;

        public static RankingReferenceEntity ReadOrDefaultAndBuild(uint rankingId, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new RankingReferenceDao()).ReadOrDefault(rankingId, readType));
        }
        public static RankingReferenceEntity CreateOrReadAndBuild(uint rankingId)
        {
            return _instantiate((new RankingReferenceDao()).CreateOrRead(rankingId));
        }

        /// <summary>
        /// 現在の参照ID。
        /// </summary>
        public byte ReferenceID => _record.ReferenceID;
        /// <summary>
        /// 次の参照ID。
        /// </summary>
        public byte NextReferenceID => (byte)(3 - ReferenceID);

        /// <summary>
        /// 参照先切り替え。他更新内容もないので Save() を内部で読んでしまう。
        /// </summary>
        /// <returns></returns>
        public bool Switch()
        {
            _setReferenceId(NextReferenceID);
            return Save();
        }
        void _setReferenceId(byte referenceId)
        {
            var newRecord = _cloneRecord();
            newRecord.ReferenceID = referenceId;
            _changeLocalRecord(newRecord);
        }
    }
}
