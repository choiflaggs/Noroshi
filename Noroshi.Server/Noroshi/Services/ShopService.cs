using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Shop;
using CharacterConstant = Noroshi.Core.Game.Character.Constant;
using ItemConstant = Noroshi.Core.Game.Item.Constant;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Shop;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Quest;

namespace Noroshi.Server.Services
{
    public class ShopService
    {
        /// <summary>
        /// ショップ一覧を取得する。
        /// </summary>
        /// <param name="playerId">取得者プレイヤー ID</param>
        /// <returns></returns>
        public static ListResponse List(uint playerId)
        {
            // プレイヤー状態をビルド。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // ショップをビルド
            var shops = ShopEntity.ReadAndBuildAllWithPlayerLevel(playerStatus.Level);
            // ショップが存在しなければ即レスポンス。
            if (shops.Count() == 0) return new ListResponse { Shops = new Shop[0] };
            // 対象プレイヤーショップ状況をビルド。
            var playerShops = PlayerShopEntity.ReadAndBuildByPlayerIDAndShops(playerId, shops);
            var shopIdToPlayerShopMap = playerShops.ToDictionary(playerShop => playerShop.ShopID);

            // 対象プレイヤーショップ状況が存在しない、もしくは商品を自動更新すべきショップを選別して商品更新。
            var updateMerchandiseShops = shops.Where(shop => !shopIdToPlayerShopMap.ContainsKey(shop.ID) || shopIdToPlayerShopMap[shop.ID].ShouldUpdateMerchandisesAutomatically(playerStatus.Level, playerStatus.VipLevel));
            if (updateMerchandiseShops.Count() > 0)
            {
                ContextContainer.ShardTransaction(tx =>
                {
                    playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                    // 商品を抽選。
                    var shopIdToMerchandiseIdsMap = updateMerchandiseShops.ToDictionary(shop => shop.ID, shop => shop.LotMerchandises(playerStatus.Level));

                    var lockPlayerShops = PlayerShopEntity.CreateOrReadAndBuildMulti(playerStatus.PlayerID, shops);
                    foreach (var playerShop in lockPlayerShops.Where(ps => ps.ShouldUpdateMerchandisesAutomatically(playerStatus.Level, playerStatus.VipLevel)))
                    {
                        playerShop.SetMerchandiseIDsAutomatically(shopIdToMerchandiseIdsMap[playerShop.ShopID].ToDictionary(kv => kv.Key, kv => kv.Value.ID));
                        if (!playerShop.Save())
                        {
                            throw new SystemException(string.Join("\t", "Fail to Save Player Shop", playerStatus.PlayerID, playerShop.ShopID));
                        }
                    }
                    tx.Commit();
                    // ロックを掛けて（作成して）取得しているプレイヤーショップは最新版なので入れ替えておく。
                    var lockPlayerShopsMap = lockPlayerShops.ToDictionary(ps => ps.ShopID);
                    playerShops = playerShops.Select(ps => lockPlayerShopsMap.ContainsKey(ps.ShopID) ? lockPlayerShopsMap[ps.ShopID] : ps);
                });
            }
            // 全ショップの商品、購入対価、更新対価 Possession をロード。
            var possessionManager = new PossessionManager(playerId, playerShops.SelectMany(playerShop => playerShop.GetAllPossessionParams()));
            possessionManager.Load();

            // 召喚石購入可否判定に利用。
            var ownCharacterSoulIdMap = SoulEntity.ReadAndBuildMultiByCharacterIDs(PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId).Select(pc => pc.CharacterID))
                .ToDictionary(pc => pc.SoulID, pc => true);

            return new ListResponse
            {
                Shops = playerShops.Select(playerShop => playerShop.ToResponseData(possessionManager, playerStatus.Level, playerStatus.VipLevel, ownCharacterSoulIdMap)).ToArray(),
            };
        }

        /// <summary>
        /// 陳列している商品を購入する。
        /// </summary>
        /// <param name="playerId">購入者プレイヤー ID</param>
        /// <param name="displayId">ショップディスプレイ ID</param>
        /// <returns></returns>
        public static BuyResponse Buy(uint playerId, uint displayId)
        {
            // プレイヤー状態をビルド。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // 陳列枠をビルド。
            var display = ShopDisplayEntity.ReadAndBuild(displayId);
            if (display == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Shop Didplay Not Found", playerId, displayId));
            }
            // ショップをビルド。
            // 条件に利用している PlayerStatusEntity はロックを掛けて取得したものではないので信用はできないが、
            // クリティカルではないので、ショップビルドはここでのみ。
            var shop = ShopEntity.ReadAndBuildWithPlayerLevel(display.ShopID, playerStatus.Level);
            if (shop == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Shop Not Found", playerId, display.ShopID));
            }
            // 召喚石購入可否判定に利用。本情報が多少遅延したところで問題ないので Slave 参照のみ。
            var ownCharacterSoulIdMap = SoulEntity.ReadAndBuildMultiByCharacterIDs(PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId).Select(pc => pc.CharacterID))
                .ToDictionary(pc => pc.SoulID, pc => true);

            // SLAVE でバリデーション。
            var response = _validateWhenBuy(playerStatus, shop, display, ownCharacterSoulIdMap, ReadType.Slave, (playerShop, merchandise, possessionManager, error) =>
            {
                return new BuyResponse { Error = error };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックをかけてバリデーションしつつ、
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                return _validateWhenBuy(playerStatus, shop, display, ownCharacterSoulIdMap, ReadType.Lock, (playerShop, merchandise, possessionManager, error) =>
                {
                    if (error != null) return new BuyResponse { Error = error };

                    // 購入。
                    playerShop.Buy(display.No);
                    if (!playerShop.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Player Shop", playerStatus.PlayerID, shop.ID));
                    }
                    // 対価支払いと商品受け取り。
                    possessionManager.Remove(merchandise.GetPaymentPossessionParam());
                    possessionManager.Add(merchandise.GetMerchandisePossessionParam());

                    // クエスト：アイテム購入回数カウントアップ。
                    QuestTriggerEntity.CountUpBuyItemShopNum(playerStatus.PlayerID);

                    tx.Commit();

                    return new BuyResponse
                    {
                        Shop = playerShop.ToResponseData(possessionManager, playerStatus.Level, playerStatus.VipLevel, ownCharacterSoulIdMap),
                    };
                });
            });
        }
        static T _validateWhenBuy<T>(PlayerStatusEntity playerStatus, ShopEntity shop, ShopDisplayEntity display, Dictionary<uint, bool> ownCharacterIdMap, ReadType readType, Func<PlayerShopEntity, ShopMerchandiseEntity, PossessionManager, ShopError, T> func)
        {
            return _validate(playerStatus, shop, readType, (playerShop, error) =>
            {
                if (error != null) return func(playerShop, null, null, error);

                // 商品を取得。
                var merchandise = playerShop.GetMerchandise(display.No);
                // 遷移上、対象商品は購入できる状況なはず（支払い内容は別）。
                if (!playerShop.CanBuy(display.No, playerStatus.Level, playerStatus.VipLevel, ownCharacterIdMap))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Buy", playerStatus.PlayerID, shop.ID, merchandise.ID, playerStatus.Level, playerStatus.VipLevel));
                }
                // 商品、対価の中身を含めた PossessionManager をインスタンス化。
                var possessionManager = new PossessionManager(playerStatus.PlayerID, playerShop.GetAllPossessionParams());
                possessionManager.Load();
                // Slave 時のみ支払えるかチェック（ロックを掛けたチェックは付与と同時に行われるため）
                if (readType == ReadType.Slave)
                {
                    if (!possessionManager.CanRemove(merchandise.GetPaymentPossessionParam()))
                    {
                        throw new InvalidOperationException(string.Join("\t", "Cannot Remove Possession", playerStatus.PlayerID, shop.ID, merchandise.ID));
                    }
                }
                return func(playerShop, merchandise, possessionManager, null);
            });
        }

        /// <summary>
        /// 陳列商品を更新する。
        /// </summary>
        /// <param name="playerId">更新者プレイヤーID</param>
        /// <param name="shopId">更新対象ショップID</param>
        /// <returns></returns>
        public static UpdateMerchandisesResponse UpdateMerchandises(uint playerId, uint shopId)
        {
            // プレイヤー状態をビルド。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            // ショップをビルド。
            // 条件に利用している PlayerStatusEntity はロックを掛けて取得したものではないので信用はできないが、
            // クリティカルではないので、ショップビルドはここでのみ。
            var shop = ShopEntity.ReadAndBuildWithPlayerLevel(shopId, playerStatus.Level);
            if (shop == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Shop Not Found", playerId, shopId));
            }
            // 召喚石購入可否判定に利用。本情報が多少遅延したところで問題ないので Slave 参照のみ。
            var ownCharacterSoulIdMap = SoulEntity.ReadAndBuildMultiByCharacterIDs(PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId).Select(pc => pc.CharacterID))
                .ToDictionary(pc => pc.SoulID, pc => true);

            // SLAVE でバリデーション。
            var response = _validateWhenUpdateMerchandisesManually(playerStatus, shop, ReadType.Slave, (playerShop, error) =>
            {
                // 支払い可否チェック。
                var possessionManager = new PossessionManager(playerId, playerShop.GetManualUpdatePossessionParam());
                possessionManager.Load();
                if (!possessionManager.CanRemove(playerShop.GetManualUpdatePossessionParam()))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Remove Possession", playerStatus.PlayerID, shop.ID));
                }
                return new UpdateMerchandisesResponse { Error = error };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックをかけてバリデーションしつつ、
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                // 商品を抽選。
                var displayNoToMerchandiseMap = shop.LotMerchandises(playerStatus.Level);
                return _validateWhenUpdateMerchandisesManually(playerStatus, shop, ReadType.Lock, (playerShop, error) =>
                {
                    if (error != null) return new UpdateMerchandisesResponse { Error = error };

                    // 更新。
                    playerShop.SetMerchandiseIDsManually(displayNoToMerchandiseMap.ToDictionary(kv => kv.Key, kv => kv.Value.ID));
                    if (!playerShop.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Player Shop", playerStatus.PlayerID, shop.ID));
                    }
                    // 更新に必要な Possession を支払う。表示に必要なパラメーターも含めておく。
                    var possessionParams = playerShop.GetPaymentAndManualUpdatePossessionParams();
                    possessionParams.AddRange(displayNoToMerchandiseMap.SelectMany(merchandise => merchandise.Value.GetAllPossessionParams()));
                    var possessionManager = new PossessionManager(playerId, possessionParams);
                    possessionManager.Load();
                    possessionManager.Remove(playerShop.GetManualUpdatePossessionParam());

                    tx.Commit();

                    return new UpdateMerchandisesResponse
                    {
                        Shop = playerShop.ToResponseData(possessionManager, playerStatus.Level, playerStatus.VipLevel, ownCharacterSoulIdMap),
                    };
                });
            });
        }
        static T _validateWhenUpdateMerchandisesManually<T>(PlayerStatusEntity playerStatus, ShopEntity shop, ReadType readType, Func<PlayerShopEntity, ShopError, T> func)
        {
            return _validate(playerStatus, shop, readType, (playerShop, error) =>
            {
                if (error != null) return func(playerShop, error);

                // 遷移上、更新できるはず。
                if (!playerShop.CanUpdateMerchandisesManually(playerStatus.Level, playerStatus.VipLevel))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Update Manually", playerStatus.PlayerID, shop.ID, playerStatus.Level, playerStatus.VipLevel));
                }
                return func(playerShop, null);
            });
        }
        static T _validate<T>(PlayerStatusEntity playerStatus, ShopEntity shop, ReadType readType, Func<PlayerShopEntity, ShopError, T> func)
        {
            // 対象プレイヤーショップ状況をビルド。
            var playerShop = PlayerShopEntity.ReadAndBuildByPlayerIDAndShop(playerStatus.PlayerID, shop, readType);
            if (playerShop == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Player Shop Not Found", playerStatus.PlayerID, shop.ID));
            }
            // 過去に開いていても、このタイミングで開いているとは限らない。
            if (!playerShop.IsOpen(playerStatus.Level, playerStatus.VipLevel))
            {
                return func(playerShop, new ShopError { ShopIsNotOpen = true });
            }
            // 自動更新タイミングもあり得る。
            if (playerShop.ShouldUpdateMerchandisesAutomatically(playerStatus.Level, playerStatus.VipLevel))
            {
                return func(playerShop, new ShopError { ShouldUpdateMerchandises = true });
            }
            return func(playerShop, null);
        }

        /// <summary>
        /// 自動売却を実行する。
        /// </summary>
        /// <param name="playerId">売却者プレイヤー ID</param>
        /// <returns></returns>
        public static SellAutomaticallyResponse SellAutomatically(uint playerId)
        {
            // プレイヤー状況をビルド。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            // 換金アイテム関連。
            var playerExchangeCashGifts = PlayerExchangeCashGiftEntity.ReadAndBuildAll(playerId);
            var exchangeCashGiftIds = playerExchangeCashGifts.Select(pecg => pecg.ExchangeCashGiftID);
            var exchangeCashGiftMap = exchangeCashGiftIds.Count() > 0 ? ExchangeCashGiftEntity.ReadAndBuildMulti(exchangeCashGiftIds).ToDictionary(ecg => ecg.ExchangeCashGiftID) : new Dictionary<uint, ExchangeCashGiftEntity>();
            // ソウルアイテム関連。
            var playerCharacters = PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId)
                .Where(pc => pc.EvolutionLevel == CharacterConstant.MAX_EVOLUTION_LEVEL);
            var souls = playerCharacters.Count() > 0 ? SoulEntity.ReadAndBuildMultiByCharacterIDs(playerCharacters.Select(pc => pc.CharacterID)) : new SoulEntity[0];
            var soulMap = souls.ToDictionary(soul => soul.SoulID);
            var playerSouls = souls.Count() > 0 ? PlayerSoulEntity.ReadAndBuildMulti(playerId, souls.Select(soul => soul.SoulID)) : new PlayerSoulEntity[0];
            // 売れるものがないエラー。
            if (exchangeCashGiftIds.Count() == 0 && souls.Count() == 0)
            {
                return new SellAutomaticallyResponse { Error = new ShopError { NoSellableItem = true } };
            }

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックを掛けて取得し直す。
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerExchangeCashGifts = exchangeCashGiftIds.Count() > 0 ? PlayerExchangeCashGiftEntity.ReadAndBuildMulti(playerId, exchangeCashGiftIds, ReadType.Lock) : new PlayerExchangeCashGiftEntity[0];
                playerSouls = souls.Count() > 0 ? PlayerSoulEntity.ReadAndBuildMulti(playerId, souls.Select(soul => soul.SoulID), ReadType.Lock) : new PlayerSoulEntity[0];

                // 売れるものがないエラー。
                if (exchangeCashGiftIds.Count() == 0 && souls.Count() == 0)
                {
                    return new SellAutomaticallyResponse { Error = new ShopError { NoSellableItem = true } };
                }
                // 付与内容算出。
                var gold = (uint)playerExchangeCashGifts.Sum(pecg => exchangeCashGiftMap[pecg.ExchangeCashGiftID].Gold * pecg.PossessionsCount);
                var soulPoint = (uint)playerSouls.Sum(ps => ps.PossessionsCount) * ItemConstant.SOUL_POINT_PER_MAX_EVOLUTION_LEVEL_CHARACTER_SOUL;
                // レスポンス用にデータを作っておく。
                var exchangeCashGiftPossessions = playerExchangeCashGifts.Select(pecg => new ExchangeCashGift(exchangeCashGiftMap[pecg.ExchangeCashGiftID], pecg.PossessionsCount, 0));
                var soulPossessions = playerSouls.Select(ps => new Soul(soulMap[ps.SoulID], ps.PossessionsCount, 0));
                // 対象換金アイテム削除。
                if (playerExchangeCashGifts.Any(pecg => !pecg.Delete()))
                {
                    throw new SystemException(string.Join("\t", "Fail to Delete Player Exchange Cash Gift", playerStatus.PlayerID));
                }
                // 対象ソウルアイテム削除。
                if (playerSouls.Any(ps => !ps.Delete()))
                {
                    throw new SystemException(string.Join("\t", "Fail to Delete Player Soul", playerStatus.PlayerID));
                }
                // 売却対価付与。
                playerStatus.AddGold(gold);
                playerStatus.AddSoulPoint(soulPoint);
                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player Status", playerStatus.PlayerID));
                }

                tx.Commit();

                return new SellAutomaticallyResponse
                {
                    Gold = gold,
                    SoulPoint = soulPoint,
                    SoldObjects = exchangeCashGiftPossessions.Select(ecg => ecg.ToResponseData()).Concat(soulPossessions.Select(s => s.ToResponseData())).ToArray(),
                };
            });
        }

        /// <summary>
        /// ショップ出現を試行する。
        /// トランザクションスコープを利用せずに更新処理をしているので、トランザクション内で呼び出すこと。
        /// </summary>
        /// <param name="playerId">試行者プレイヤーID</param>
        /// <param name="playerLevel">試行者プレイヤーレベル</param>
        /// <param name="vipLevel">試行者 VIP レベル</param>
        /// <returns></returns>
        public static ShopEntity TryToAppear(uint playerId, ushort playerLevel, ushort vipLevel)
        {
            var shopCandidates = ShopEntity.ReadAndBuildAppearanceCandidates(playerLevel, vipLevel);
            foreach (var shop in shopCandidates)
            {
                if (ContextContainer.GetContext().RandomGenerator.Lot(shop.AppearProbability))
                {
                    var playerShop = PlayerShopEntity.TryToAppear(playerId, playerLevel, vipLevel, shop);
                    if (playerShop != null) return shop;
                }
            }
            return null;
        }
    }
}
