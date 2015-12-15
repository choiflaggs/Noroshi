using System.Collections.Generic;
using System;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerShopSchema;

namespace Noroshi.Server.Entity.Shop
{
    public class PlayerShopEntity : AbstractDaoWrapperEntity<PlayerShopEntity, PlayerShopDao, Schema.PrimaryKey, Schema.Record>
    {
        const ushort DEFAULT_MAX_MERCHANDISE_MANUAL_UPDATE_NUM = 3;

        /// <summary>
        /// プレイヤー ID とショップでビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="shop">対象ショップ</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerShopEntity ReadAndBuildByPlayerIDAndShop(uint playerId, ShopEntity shop, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildByPlayerIDAndShops(playerId, new ShopEntity[] { shop }, readType).FirstOrDefault();
        }
        /// <summary>
        /// プレイヤー ID とショップでビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="shops">対象ショップ</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static IEnumerable<PlayerShopEntity> ReadAndBuildByPlayerIDAndShops(uint playerId, IEnumerable<ShopEntity> shops, ReadType readType = ReadType.Slave)
        {
            return _setShops(ReadAndBuildMulti(shops.Select(shop => new Schema.PrimaryKey { PlayerID = playerId, ShopID = shop.ID }), readType), shops);
        }
        /// <summary>
        /// レコード作成を試みた上で、ロックを掛けてビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="shop">対象ショップ</param>
        /// <returns></returns>
        public static PlayerShopEntity CreateOrReadAndBuild(uint playerId, ShopEntity shop)
        {
            return CreateOrReadAndBuildMulti(playerId, new ShopEntity[] { shop }).FirstOrDefault();
        }
        /// <summary>
        /// レコード作成を試みた上で、ロックを掛けてビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="shops">対象ショップ</param>
        /// <returns></returns>
        public static IEnumerable<PlayerShopEntity> CreateOrReadAndBuildMulti(uint playerId, IEnumerable<ShopEntity> shops)
        {
            return _setShops(_instantiate((new PlayerShopDao()).CreateOrReadMulti(playerId, shops.Select(shop => shop.ID))), shops);
        }

        /// <summary>
        /// ショップ出現を試行する。
        /// </summary>
        /// <param name="playerId">出現試行者プレイヤー ID</param>
        /// <param name="playerLevel">出現試行者プレイヤーレベル</param>
        /// <param name="vipLevel">出現試行者 VIP レベル</param>
        /// <param name="shop">対象ショップ</param>
        public static PlayerShopEntity TryToAppear(uint playerId, ushort playerLevel, ushort vipLevel, ShopEntity shop)
        {
            // 臨時ショップが渡されることはないはず。
            if (!shop.IsTemporaryShop) throw new InvalidOperationException();

            var entity = CreateOrReadAndBuild(playerId, shop);
            // 既に開いているなら対象外。
            if (entity.IsOpen(playerLevel, vipLevel)) return null;
            entity._appear();
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Shop", playerId, entity.ShopID));
            }
            return entity;
        }
        public static IEnumerable<PlayerShopEntity> _setShops(IEnumerable<PlayerShopEntity> entities, IEnumerable<ShopEntity> shops)
        {
            var shopMap = shops.ToDictionary(shop => shop.ID);
            var shopIdToEntityMap = entities.Select(entity =>
            {
                entity._setShop(shopMap[entity.ShopID]);
                return entity;
            })
            .ToDictionary(entity => entity.ShopID);
            return shops.Where(shop => shopIdToEntityMap.ContainsKey(shop.ID)).Select(shop => shopIdToEntityMap[shop.ID]);
        }


        ShopEntity _shop;

        void _setShop(ShopEntity shop)
        {
            if (_record.ShopID != shop.ID) throw new InvalidOperationException();
            _shop = shop;
        }


        public uint ShopID => _record.ShopID;
        uint? _appearedAt => _record.AppearedAt > 0 ? (uint?)_record.AppearedAt : null;

        /// <summary>
        /// オープンチェック。
        /// </summary>
        /// <param name="playerLevel">判定者プレイヤーレベル</param>
        /// <param name="vipLevel">判定者 VIP レベル</param>
        public bool IsOpen(ushort playerLevel, ushort vipLevel)
        {
            // 定常化していたら常にオープン。
            if (_shop.IsResident(playerLevel, vipLevel)) return true;

            // 出現履歴があれば出現時間内か判定。
            var current = ContextContainer.GetContext().TimeHandler.UnixTime;
            if (_appearedAt.HasValue && current < _appearedAt.Value + (uint)_shop.AppearTime.Value.TotalSeconds)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 次の商品定期更新時間を取得。
        /// </summary>
        public DateTime GetNextMerchandiseScheduledUpdateTime()
        {
            return ContextContainer.GetContext().TimeHandler.SelectNextResetDateTime(_shop.GetMerchandiseScheduledUpdateHours());
        }
        /// <summary>
        /// 陳列商品を取得する。
        /// </summary>
        /// <param name="displayNo">対象ディスプレイ番号</param>
        /// <returns></returns>
        public ShopMerchandiseEntity GetMerchandise(byte displayNo)
        {
            return _shop.GetMerchandise(_getDisplayNoToMerchandiseIDMap()[displayNo]);
        }
        /// <summary>
        /// 陳列している全商品を取得する。
        /// </summary>
        public Dictionary<byte, ShopMerchandiseEntity> GetMerchandises()
        {
            return _getDisplayNoToMerchandiseIDMap().ToDictionary(kv => kv.Key, kv => _shop.GetMerchandise(kv.Value));
        }
        Dictionary<byte, uint> _getDisplayNoToMerchandiseIDMap()
        {
            return _deserializeFromText<Dictionary<byte, uint>>(_record.Merchandises) ?? new Dictionary<byte, uint>();
        }
        byte[] _getBoughtDisplayNos()
        {
            return _deserializeFromText<byte[]>(_record.BoughtDisplayNos) ?? new byte[0];
        }

        /// <summary>
        /// 陳列している全商品の商品、購入対価と手動更新対価とショップとしての（主な）対価を合わせた PossessionParam を取得。
        /// </summary>
        public List<PossessionParam> GetAllPossessionParams()
        {
            var possessionParams = GetMerchandises().Values.SelectMany(merchandise => merchandise.GetAllPossessionParams()).ToList();
            possessionParams.AddRange(GetPaymentAndManualUpdatePossessionParams());
            return possessionParams;
        }
        public List<PossessionParam> GetPaymentAndManualUpdatePossessionParams()
        {
            var possessionParams = new List<PossessionParam>();
            possessionParams.Add(GetManualUpdatePossessionParam());
            possessionParams.Add(_shop.GetPaymentPossessionParam());
            return possessionParams;
        }

        /// <summary>
        /// 商品自動更新をすべきか判定。
        /// </summary>
        /// <param name="playerLevel">プレイヤーレベル</param>
        /// <param name="vipLevel">VIP レベル</param>
        public bool ShouldUpdateMerchandisesAutomatically(ushort playerLevel, ushort vipLevel)
        {
            if (!IsOpen(playerLevel, vipLevel)) return false;
            return _shouldUpdateMerchandisesOnSchedule() || _getDisplayNoToMerchandiseIDMap().Count() == 0;
        }
        bool _shouldUpdateMerchandisesOnSchedule()
        {
            return ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.MerchandiseUpdatedAtOnSchedule, _shop.GetMerchandiseScheduledUpdateHours());
        }
        /// <summary>
        /// 商品自動更新によるセット。
        /// </summary>
        /// <param name="displayNoToMerchandiseId">陳列商品（DisplayNo => MerchandiseID）</param>
        public void SetMerchandiseIDsAutomatically(Dictionary<byte, uint> displayNoToMerchandiseId)
        {
            var newRecord = _cloneRecord();
            if (_shouldUpdateMerchandisesOnSchedule()) newRecord.MerchandiseUpdatedAtOnSchedule = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            _setMerchandiseIDs(displayNoToMerchandiseId);
        }
        /// <summary>
        /// デイリー最大手動商品更新回数を取得。
        /// </summary>
        /// <param name="vipLevel">VIP レベル</param>
        public ushort GetMaxMerchandiseManualUpdateNum(ushort vipLevel)
        {
            // TODO : 仕様待ち。
            return (ushort)(DEFAULT_MAX_MERCHANDISE_MANUAL_UPDATE_NUM + vipLevel - 1);
        }
        /// <summary>
        /// 手動商品更新可否チェック。
        /// </summary>
        /// <param name="playerLevel">更新者プレイヤーレベル</param>
        /// <param name="vipLevel">更新者 VIP レベル</param>
        /// <returns></returns>
        public bool CanUpdateMerchandisesManually(ushort playerLevel, ushort vipLevel)
        {
            if (!IsOpen(playerLevel, vipLevel)) return false;
            return GetCurrentMerchandiseManualUpdateNum() < GetMaxMerchandiseManualUpdateNum(vipLevel);
        }
        /// <summary>
        /// 手動更新対価を取得する。
        /// </summary>
        /// <returns></returns>
        public PossessionParam GetManualUpdatePossessionParam()
        {
            return _shop.GetMerchandiseManualUpdatePayment(GetCurrentMerchandiseManualUpdateNum());
        }
        /// <summary>
        /// 手動更新回数取得。
        /// </summary>
        /// <returns></returns>
        public ushort GetCurrentMerchandiseManualUpdateNum()
        {
            return ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.MerchandiseUpdatedAtManually) ? (ushort)0 : _record.MerchandiseManualUpdateNum;
        }
        /// <summary>
        /// 商品手動更新によるセット。
        /// </summary>
        /// <param name="displayNoToMerchandiseId">陳列商品（DisplayNo => MerchandiseID）</param>
        public void SetMerchandiseIDsManually(Dictionary<byte, uint> displayNoToMerchandiseId)
        {
            var newRecord = _cloneRecord();
            newRecord.MerchandiseManualUpdateNum = (ushort)(GetCurrentMerchandiseManualUpdateNum() + 1);
            newRecord.MerchandiseUpdatedAtManually = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            _setMerchandiseIDs(displayNoToMerchandiseId);
        }
        void _setMerchandiseIDs(Dictionary<byte, uint> displayNoToMerchandiseId)
        {
            var newRecord = _cloneRecord();
            newRecord.Merchandises = _serializeToText(displayNoToMerchandiseId);
            newRecord.BoughtDisplayNos = "";
            _changeLocalRecord(newRecord);
        }

        void _appear()
        {
            var newRecord = _cloneRecord();
            // このタイミングでは商品を空にしておく（ショップアクセス時に入荷する）。
            newRecord.Merchandises = "";
            newRecord.BoughtDisplayNos = "";
            newRecord.AppearedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
        }

        /// <summary>
        /// 購入可否チェック。
        /// </summary>
        /// <param name="displayNo">ディスプレイ番号</param>
        /// <param name="playerLevel">購入者プレイヤーレベル</param>
        /// <param name="vipLevel">購入者 VIP レベル</param>
        /// <param name="ownCharacterSoulIdMap">保有キャラクターに紐づく召喚石 ID マップ（召喚石 ID => true）</param>
        /// <returns></returns>
        public bool CanBuy(byte displayNo, ushort playerLevel, ushort vipLevel, Dictionary<uint, bool> ownCharacterSoulIdMap)
        {
            if (!IsOpen(playerLevel, vipLevel)) return false;
            // 自動更新時に購入は不可。
            if (ShouldUpdateMerchandisesAutomatically(playerLevel, vipLevel)) return false;
            // 召喚石はキャラクターを獲得していないと不可。
            if (_isClosedSoul(displayNo, ownCharacterSoulIdMap)) return false;
            return _getDisplayNoToMerchandiseIDMap().ContainsKey(displayNo) && !_hasAlreadyBought(displayNo);
        }
        bool _hasAlreadyBought(byte displayNo)
        {
            return _getBoughtDisplayNos().Any(id => id == displayNo);
        }
        bool _isClosedSoul(byte displayNo, Dictionary<uint, bool> ownCharacterSoulIdMap)
        {
            var merchandisePossessionParam = GetMerchandise(displayNo).GetMerchandisePossessionParam();
            if (merchandisePossessionParam.Category == PossessionCategory.Soul)
            {
                if (!ownCharacterSoulIdMap.ContainsKey(merchandisePossessionParam.ID)) return true;
            }
            return false;
        }
        /// <summary>
        /// 購入する。
        /// </summary>
        /// <param name="displayNo">ディスプレイ番号</param>
        public void Buy(byte displayNo)
        {
            var boughtDisplayNos = _getBoughtDisplayNos().ToList();
            boughtDisplayNos.Add(displayNo);
            var newRecord = _cloneRecord();
            newRecord.BoughtDisplayNos = _serializeToText(boughtDisplayNos.ToArray());
            _changeLocalRecord(newRecord);
        }

        public Core.WebApi.Response.Shop.Shop ToResponseData(PossessionManager possessionManager, ushort playerLevel,  ushort vipLevel, Dictionary<uint, bool> ownCharacterSoulIdMap)
        {
            return new Core.WebApi.Response.Shop.Shop
            {
                ID = _shop.ID,
                TextKey = _shop.TextKey,
                IsOpen = IsOpen(playerLevel, vipLevel),
                Merchandises = GetMerchandises().Select(kv =>
                {
                    var displayNo = kv.Key;
                    var merchandise = kv.Value;
                    return new Core.WebApi.Response.Shop.Merchandise
                    {
                        ID = merchandise.ID,
                        DisplayID = _shop.GetDisplayID(displayNo),
                        DisplayNo = displayNo,
                        MerchandisePossessionObject = possessionManager.GetPossessionObject(merchandise.GetMerchandisePossessionParam()).ToResponseData(),
                        PaymentPossessionObject = possessionManager.GetPossessionObject(merchandise.GetPaymentPossessionParam()).ToResponseData(),
                        HasAlreadyBought = _hasAlreadyBought(displayNo),
                        IsClosedSoul = _isClosedSoul(displayNo, ownCharacterSoulIdMap),
                        CanBuy = CanBuy(displayNo, playerLevel, vipLevel, ownCharacterSoulIdMap) && possessionManager.CanRemove(merchandise.GetPaymentPossessionParam()),
                    };
                }).ToArray(),
                NextMerchandiseScheduledUpdateTime = _shop.IsResident(playerLevel, vipLevel) ? (DateTime?)GetNextMerchandiseScheduledUpdateTime() : null,
                CanUpdateMerchandisesManually = CanUpdateMerchandisesManually(playerLevel, vipLevel) && possessionManager.CanRemove(GetManualUpdatePossessionParam()),
                CurrentMerchandiseManualUpdateNum = GetCurrentMerchandiseManualUpdateNum(),
                MaxMerchandiseManualUpdateNum = GetMaxMerchandiseManualUpdateNum(vipLevel),
                ManualUpdatePossessionObject = possessionManager.GetPossessionObject(GetManualUpdatePossessionParam()).ToResponseData(),
                PaymentPossessionObject = possessionManager.GetPossessionObject(_shop.GetPaymentPossessionParam()).ToResponseData(),
            };
        }
    }
}
