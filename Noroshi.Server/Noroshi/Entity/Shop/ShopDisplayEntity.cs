using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopDisplaySchema;

namespace Noroshi.Server.Entity.Shop
{
    /// <summary>
    /// 商品が陳列する枠情報を扱うクラス。
    /// </summary>
    public class ShopDisplayEntity : AbstractDaoWrapperEntity<ShopDisplayEntity, ShopDisplayDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="id">ショップディスプレイ ID</param>
        /// <returns></returns>
        public static ShopDisplayEntity ReadAndBuild(uint id)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = id }));
        }
        /// <summary>
        /// ショップ ID を指定してビルドする。
        /// </summary>
        /// <param name="shopIds">ショップ ID</param>
        /// <returns></returns>
        public static IEnumerable<ShopDisplayEntity> ReadAndBuildByShopIDs(IEnumerable<uint> shopIds)
        {
            return _loadAssociatedEntities(_instantiate((new ShopDisplayDao()).ReadByShopIDs(shopIds)));
        }
        static ShopDisplayEntity _loadAssociatedEntities(ShopDisplayEntity entity)
        {
            if (entity == null) return entity;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<ShopDisplayEntity> _loadAssociatedEntities(IEnumerable<ShopDisplayEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var shopIdToMerchandises = ShopMerchandiseEntity.ReadAndBuildByMerchandiseGroupIDs(entities.Select(e => e.MerchandiseGroupID)).ToLookup(merchandise => merchandise.MerchandiseGroupID);
            return entities.Select(entity =>
            {
                entity._setMerchandises(shopIdToMerchandises[entity.MerchandiseGroupID]);
                return entity;
            });
        }


        Dictionary<uint, ShopMerchandiseEntity> _merchandiseMap;

        void _setMerchandises(IEnumerable<ShopMerchandiseEntity> merchandises)
        {
            _merchandiseMap = merchandises.ToDictionary(merchandise => merchandise.ID);
        }

        /// <summary>
        /// ディスプレイ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// ショップ ID。
        /// </summary>
        public uint ShopID => _record.ShopID;
        /// <summary>
        /// 陳列枠番号。
        /// </summary>
        public byte No => _record.No;
        /// <summary>
        ///  商品陳列時に利用する商品グループ ID。
        /// </summary>
        public uint MerchandiseGroupID => _record.MerchandiseGroupID;

        /// <summary>
        /// 抽選可能な商品を取得する。
        /// </summary>
        /// <param name="playerLevel">抽選者プレイヤーレベル</param>
        /// <returns></returns>
        public IEnumerable<ShopMerchandiseEntity> GetLottableMerchandises(ushort playerLevel)
        {
            return _merchandiseMap.Values;
        }

        /// <summary>
        /// 陳列のために商品を抽選する。
        /// </summary>
        /// <param name="playerLevel">抽選者プレイヤーレベル</param>
        public ShopMerchandiseEntity Lot(ushort playerLevel)
        {
            var candidates = _merchandiseMap.Values.Where(merchandise => merchandise.IsLotCandidate(playerLevel));
            return ContextContainer.GetContext().RandomGenerator.Lot(candidates, merchandise => merchandise.Weight);
        }
    }
}
