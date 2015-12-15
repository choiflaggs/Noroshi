using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Gacha;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GachaSchema;

namespace Noroshi.Server.Entity.Gacha
{
    /// <summary>
    /// ガチャ処理のコアな部分のみを扱うクラス。
    /// </summary>
    public class GachaEntity : AbstractDaoWrapperEntity<GachaEntity, GachaDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="id">ガチャ ID</param>
        /// <returns></returns>
        public static GachaEntity ReadAndBuild(uint id)
        {
            return ReadAndBuildMulti(new[] { id }).FirstOrDefault();
        }
        /// <summary>
        /// 複数ビルド。
        /// </summary>
        /// <param name="ids">ガチャ ID</param>
        /// <returns></returns>
        public static IEnumerable<GachaEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            var entities = ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey() { ID = id }));
            var gachaIdToContentsMap = GachaContentEntity.ReadAndBuildMultiByGachaIDs(ids).ToLookup(gc => gc.GachaID);
            return entities.Select(entity =>
            {
                entity._setContent(gachaIdToContentsMap[entity.ID]);
                return entity;
            });
        }


        IEnumerable<GachaContentEntity> _contents;

        void _setContent(IEnumerable<GachaContentEntity> contents)
        {
            if (contents.Any(content => content.GachaID != ID)) throw new InvalidOperationException();
            _contents = contents.OrderBy(ic => ic.Weight);
        }


        /// <summary>
        /// ガチャ ID。
        /// </summary>
        public uint ID => _record.ID;

        /// <summary>
        /// 抽選し得る Possession Param を取得。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetPossessionParamCandidates()
        {
            return _contents.Select(content => content.GetPossessableParam());
        }

        /// <summary>
        /// 抽選する。
        /// </summary>
        /// <param name="lotNum">抽選回数</param>
        /// <returns></returns>
        public List<GachaContentEntity> Lot(byte lotNum)
        {
            return Lot(lotNum, (content, no) => true);
        }

        /// <summary>
        /// 抽選する。ただし、抽選対象フィルタリング付き。
        /// </summary>
        /// <param name="lotNum">抽選回数</param>
        /// <param name="contentFilter">抽選対象フィルター</param>
        /// <returns></returns>
        public List<GachaContentEntity> Lot(byte lotNum, Func<GachaContentEntity, byte, bool> contentFilter)
        {
            var candidates = _contents.Where((content, i) => contentFilter(content, (byte)(i + 1)));
            if (candidates.Count() == 0) throw new SystemException();
            var lottedContents = new List<GachaContentEntity>();
            for (var i = 0; i < lotNum; i++)
            {
                lottedContents.Add(ContextContainer.GetContext().RandomGenerator.Lot(candidates, content => content.Weight));
            }
            return lottedContents;
        }
    }
}
