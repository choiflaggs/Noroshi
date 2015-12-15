using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopMerchandiseSchema;

namespace Noroshi.Server.Entity.Shop
{
    /// <summary>
    /// 商品情報を扱うクラス。
    /// </summary>
    public class ShopMerchandiseEntity : AbstractDaoWrapperEntity<ShopMerchandiseEntity, ShopMerchandiseDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 商品グループ ID でビルドする。
        /// </summary>
        /// <param name="groupIds">商品グループ ID</param>
        /// <returns></returns>
        public static IEnumerable<ShopMerchandiseEntity> ReadAndBuildByMerchandiseGroupIDs(IEnumerable<uint> groupIds)
        {
            return _instantiate((new ShopMerchandiseDao()).ReadByMerchandiseGroupIDs(groupIds));
        }

        /// <summary>
        /// 商品 ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 商品グループ ID。
        /// </summary>
        public uint MerchandiseGroupID => _record.MerchandiseGroupID;
        /// <summary>
        /// 抽選時の重み。
        /// </summary>
        public uint Weight => _record.Weight;

        /// <summary>
        /// 商品陳列時の抽選対象かどうか。
        /// </summary>
        /// <param name="playerLevel">抽選者プレイヤーレベル</param>
        /// <returns></returns>
        public bool IsLotCandidate(ushort playerLevel)
        {
            return _record.MinPlayerLevel <= playerLevel && playerLevel <= _record.MaxPlayerLevel;
        }

        /// <summary>
        /// 商品 Possession パラメーター。
        /// </summary>
        /// <returns></returns>
        public PossessionParam GetMerchandisePossessionParam()
        {
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.MerchandisePossessionCategory,
                ID = _record.MerchandisePossessionID,
                Num = _record.MerchandisePossessionNum,
            };
        }
        /// <summary>
        /// 価格 Possession パラメーター。
        /// </summary>
        /// <returns></returns>
        public PossessionParam GetPaymentPossessionParam()
        {
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.PaymentPossessionCategory,
                ID = _record.PaymentPossessionID,
                Num =_record.PaymentPossessionNum,
            };
        }
        /// <summary>
        /// 商品 Possession パラメータ、価格 Possession パラメーターを合わせて返す。
        /// </summary>
        public PossessionParam[] GetAllPossessionParams()
        {
            return new PossessionParam[]
            {
                GetMerchandisePossessionParam(),
                GetPaymentPossessionParam(),
            };
        }
    }
}
