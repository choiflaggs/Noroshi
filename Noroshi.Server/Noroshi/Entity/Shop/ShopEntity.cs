using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShopSchema;

namespace Noroshi.Server.Entity.Shop
{
    /// <summary>
    /// ショップ情報を扱うクラス。
    /// </summary>
    public class ShopEntity : AbstractDaoWrapperEntity<ShopEntity, ShopDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// リセット時間設定
        /// </summary>
        static readonly Dictionary<byte, byte[]> MERCHANDISE_SCHEDULED_UPDATE_HOURS_MAP = new Dictionary<byte, byte[]>
        {
            {1, new byte[] { Constant.DAILY_RESET_HOUR }},
            {4, new byte[] { 9, 12, 18, 21 }},
        };

        /// <summary>
        /// プレイヤーレベルによるフィルタリングありでビルド。
        /// </summary>
        /// <param name="shopId">ショップ ID</param>
        /// <param name="playerLevel">プレイヤーレベル</param>
        /// <returns></returns>
        public static ShopEntity ReadAndBuildWithPlayerLevel(uint shopId, ushort playerLevel)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = shopId }), playerLevel);
        }
        /// <summary>
        /// プレイヤーレベルによるフィルタリングありで全てビルド。
        /// </summary>
        /// <param name="playerLevel">プレイヤーレベル</param>
        /// <returns></returns>
        public static IEnumerable<ShopEntity> ReadAndBuildAllWithPlayerLevel(ushort playerLevel)
        {
            return _filterAndLoadAssociatedEntities(_instantiate((new ShopDao()).ReadAll()), playerLevel);
        }
        /// <summary>
        /// 出現候補となるショップをビルド。
        /// </summary>
        /// <param name="playerLevel">抽選者プレイヤーレベル</param>
        /// <param name="vipLevel">抽選者 VIP レベル</param>
        public static IEnumerable<ShopEntity> ReadAndBuildAppearanceCandidates(ushort playerLevel, ushort vipLevel)
        {
            return _filterAndLoadAssociatedEntities(_instantiate((new ShopDao()).ReadNotResidentShops(playerLevel, vipLevel)), playerLevel);
        }
        static ShopEntity _loadAssociatedEntities(ShopEntity entity, ushort playerLevel)
        {
            if (entity == null) return entity;
            return _filterAndLoadAssociatedEntities(new[] { entity }, playerLevel).FirstOrDefault();
        }

        /// <summary>
        /// VIPレベルがあるものを全てビルド。
        /// </summary>
        public static IEnumerable<ShopEntity> ReadAndBuildTemporaryShops()
        {
            return _instantiate((new ShopDao()).ReadMoreThanVipLevel(Core.Game.Player.Constant.MIN_VIP_LEVEL));
        }

        static IEnumerable<ShopEntity> _filterAndLoadAssociatedEntities(IEnumerable<ShopEntity> entities, ushort playerLevel)
        {
            var openGameContentMap = Core.Game.GameContent.GameContent.BuildOpenGameContentsByPlayerLevel(playerLevel)
                .ToDictionary(gc => gc.ID);
            var filteredEntities = entities.Where(entity => !(entity.RelatedGameContentID.HasValue && !openGameContentMap.ContainsKey(entity.RelatedGameContentID.Value)));
            if (filteredEntities.Count() == 0) return filteredEntities;
            var shopIdToDisplays = ShopDisplayEntity.ReadAndBuildByShopIDs(filteredEntities.Select(e => e.ID)).ToLookup(display => display.ShopID);
            return filteredEntities.Select(entity =>
            {
                entity._setDisplays(shopIdToDisplays[entity.ID], playerLevel);
                return entity;
            });
        }


        Dictionary<byte, ShopDisplayEntity> _displayNoTodisplayMap;
        Dictionary<uint, ShopMerchandiseEntity> _merchandiseMap;

        void _setDisplays(IEnumerable<ShopDisplayEntity> displays, ushort playerLevel)
        {
            _displayNoTodisplayMap = displays.ToDictionary(display => display.No);
            _merchandiseMap = displays.SelectMany(display => display.GetLottableMerchandises(playerLevel))
                .ToLookup(merchandise => merchandise.ID)
                .ToDictionary(idToMerchandises => idToMerchandises.Key, idToMerchandises => idToMerchandises.First());
        }


        /// <summary>
        /// ショップ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey => "Master.Shop." + _record.TextKey;
        /// <summary>
        /// 関係ゲームコンテンツ ID。
        /// </summary>
        public uint? RelatedGameContentID => _record.RelatedGameContentID > 0 ? (uint?)_record.RelatedGameContentID : null;
        /// <summary>
        /// 出現確率。
        /// </summary>
        public float AppearProbability => _record.AppearProbability;
        /// <summary>
        /// 出現期間。
        /// </summary>
        public TimeSpan? AppearTime => _record.AppearMinute > 0 ? (TimeSpan?)TimeSpan.FromMinutes(_record.AppearMinute) : null;
        /// <summary>
        /// 臨時ショップフラグ。
        /// プレイヤー状態により定常ショップ化した場合も真であることに注意。
        /// </summary>
        public bool IsTemporaryShop => _record.ResidentVipLevel > 0;
        /// <summary>
        /// 常設されるVIPレベル
        /// </summary>
        public ushort ResidentVipLevel => _record.ResidentVipLevel;

        /// <summary>
        /// ディスプレイ ID を取得する。
        /// </summary>
        /// <param name="displayNo">陳列番号</param>
        /// <returns></returns>
        public uint GetDisplayID(byte displayNo)
        {
            return _displayNoTodisplayMap[displayNo].ID;
        }
        /// <summary>
        /// 商品を取得。
        /// </summary>
        /// <param name="merchandiseId">対象商品 ID</param>
        /// <returns></returns>
        public ShopMerchandiseEntity GetMerchandise(uint merchandiseId)
        {
            return GetMerchandises(new[] { merchandiseId }).FirstOrDefault();
        }
        /// <summary>
        /// 複数商品を取得。
        /// </summary>
        /// <param name="merchandiseIds">対象商品ID</param>
        /// <returns></returns>
        public IEnumerable<ShopMerchandiseEntity> GetMerchandises(IEnumerable<uint> merchandiseIds)
        {
            return merchandiseIds.Select(merchandiseId => _merchandiseMap[merchandiseId]);
        }

        /// <summary>
        /// 定常ショップかどうかチェック。
        /// 臨時ショップであってもプレイヤー状態によっては定常化することに注意。
        /// </summary>
        /// <param name="playerLevel">判定者レイヤーレベル</param>
        /// <param name="vipLevel">判定者 VIP レベル</param>
        public bool IsResident(ushort playerLevel, ushort vipLevel)
        {
            if (!IsTemporaryShop) return true;
            return _record.ResidentVipLevel <= vipLevel && _record.AppearPlayerLevel <= playerLevel;
        }

        /// <summary>
        /// 商品定期入れ替え時を取得。
        /// </summary>
        public byte[] GetMerchandiseScheduledUpdateHours()
        {
            return MERCHANDISE_SCHEDULED_UPDATE_HOURS_MAP[_record.DailyScheduledUpdateNum];
        }

        /// <summary>
        /// 商品手動入れ替え対価を取得する。
        /// </summary>
        /// <param name="manualUpdateNum">本日、既に商品手動入れ替えした回数</param>
        public PossessionParam GetMerchandiseManualUpdatePayment(ushort manualUpdateNum)
        {
            var paymentCalculator = new Core.Game.Player.RepeatablePaymentCalculator(_record.ManualUpdatePossessionNum);
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.ManualUpdatePossessionCategory,
                ID = _record.ManualUpdatePossessionID,
                Num = paymentCalculator.GetPaymentNum(manualUpdateNum),
            };
        }
        /// <summary>
        /// （主に）支払いに利用される Possession パラメーターを取得。Num が 0 であることに注意。
        /// </summary>
        /// <returns></returns>
        public PossessionParam GetPaymentPossessionParam()
        {
            return new PossessionParam
            {
                Category = (PossessionCategory)_record.PaymentPossessionCategory,
                ID = _record.PaymentPossessionID,
                Num = 0, // Dummy
            };
        }

        /// <summary>
        /// 陳列のために商品を抽選する。
        /// </summary>
        /// <param name="playerLevel">抽選者プレイヤーレベル</param>
        public Dictionary<byte, ShopMerchandiseEntity> LotMerchandises(ushort playerLevel)
        {
            return _displayNoTodisplayMap.ToDictionary(kv => kv.Key, kv => kv.Value.Lot(playerLevel));
        }
    }
}
