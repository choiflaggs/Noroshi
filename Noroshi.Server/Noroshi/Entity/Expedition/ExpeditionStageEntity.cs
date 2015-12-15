using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Expedition;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExpeditionStageSchema;

namespace Noroshi.Server.Entity.Expedition
{
    /// <summary>
    /// 冒険ステージ設定を扱うクラス。
    /// </summary>
    public class ExpeditionStageEntity : AbstractDaoWrapperEntity<ExpeditionStageEntity, ExpeditionStageDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 指定冒険に紐付く冒険ステージをビルドする。
        /// </summary>
        /// <param name="expeditionIds">対象冒険 ID</param>
        /// <returns></returns>
        public static IEnumerable<ExpeditionStageEntity> ReadAndBuildByExpeditionIDs(IEnumerable<uint> expeditionIds)
        {
            return _instantiate((new ExpeditionStageDao()).ReadByExpeditionIDs(expeditionIds));
        }

        /// <summary>
        /// 冒険ステージ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 冒険 ID。
        /// </summary>
        public uint ExpeditionID => _record.ExpeditionID;
        /// <summary>
        /// ステップ。所属冒険において何番目のステージかを表す。
        /// </summary>
        public byte Step => _record.Step;
        /// <summary>
        /// （理想的な）対戦相手最大プレイヤーレベル。
        /// </summary>
        public ushort MaxPlayerLevel => _record.MaxPlayerLevel;
        /// <summary>
        /// （理想的な）対戦相手最小プレイヤーレベル。
        /// </summary>
        public ushort MinPlayerLevel => _record.MinPlayerLevel;
        /// <summary>
        /// 対応宝箱において獲得できるゴールド。
        /// </summary>
        public uint Gold => _record.Gold;
        /// <summary>
        /// 対応宝箱において獲得できる冒険ポイント。
        /// </summary>
        public uint ExpeditionPoint => _record.ExpeditionPoint;
        /// <summary>
        /// 対応宝箱において抽選対象となるガチャ ID。
        /// </summary>
        public uint GachaID => _record.GachaID;
        /// <summary>
        /// 対応宝箱においてガチャを抽選する回数。
        /// </summary>
        public byte GachaLotNum => _record.GachaLotNum;
    }
}
