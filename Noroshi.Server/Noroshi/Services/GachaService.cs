using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Gacha;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Gacha;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Quest;

namespace Noroshi.Server.Services
{
    public class GachaService
    {
        public static EntryPointListResponse EntryPointList(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            var openGameContentMap = playerStatus.GetOpenGameContents().ToDictionary(gameContent => gameContent.ID);

            // 開放されているエントリーポイントをビルド。
            var entryPoints = GachaEntryPointEntity.ReadAndBuildAllWithoutTutorialEntryPoint()
                .Where(entryPoint => entryPoint.IsOpen() && (!entryPoint.GameContentID.HasValue || openGameContentMap.ContainsKey(entryPoint.GameContentID.Value)));
            // 併せてプレイヤー状況もビルド。
            var playerEntryPointMap = entryPoints.Count() > 0
                ? PlayerGachaEntryPointEntity.ReadOrDefaultBuildByPlayerIDAndGachaEntryPointIDs(playerId, entryPoints.Select(entryPoint => entryPoint.ID))
                    .ToDictionary(playerEntryPoint => playerEntryPoint.GachaEntryPointID)
                : new Dictionary<uint, PlayerGachaEntryPointEntity>();

            // Possession をロード。
            var possessionParams = entryPoints.SelectMany(entryPoint => entryPoint.GetAllPossessionParams());
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            // レスポンス。
            return new EntryPointListResponse
            {
                GachaEntryPoints = entryPoints.Select(entryPoint => entryPoint.ToResponseData(playerEntryPointMap[entryPoint.ID], possessionManager)).ToArray(),
            };
        }
        public static LotResponse Lot(uint playerId, uint gachaEntryPointId, bool free)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            var openGameContentMap = playerStatus.GetOpenGameContents().ToDictionary(gameContent => gameContent.ID);

            // エントリーポイントをビルド。
            var entryPoint = GachaEntryPointEntity.ReadAndBuildWithoutTutorialEntryPoint(gachaEntryPointId);
            if (entryPoint == null || !entryPoint.IsOpen() || (entryPoint.GameContentID.HasValue && !openGameContentMap.ContainsKey(entryPoint.GameContentID.Value)))
            {
                return new LotResponse { Error = new GachaError { EntryPointNotFound = true } };
            }
            // PossessionManager ロード。
            var possessionManager = _checkAndGetPossessionManager(playerId, entryPoint, free);

            // トランザクション
            return ContextContainer.ShardTransaction(tx =>
            {
                return _lot(playerId, entryPoint, free, possessionManager, (playerGachaEntryPoint, lottedContentPossessionParams, error) =>
                {
                    if (error != null) return new LotResponse { Error = error };

                    tx.Commit();

                    return new LotResponse
                    {
                        GachaEntryPoint = entryPoint.ToResponseData(playerGachaEntryPoint, possessionManager),
                        LotPossessionObjects = possessionManager.GetPossessionObjects(lottedContentPossessionParams).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }
        public static LotResponse LotTutorialGacha(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (!playerStatus.CanLotTutorialGacha())
            {
                throw new InvalidOperationException("Cannot Lot Tutorial Gacha");
            }
            // エントリーポイントをビルド。
            var entryPoint = GachaEntryPointEntity.ReadAndBuildTutorialEntryPoint();
            if (entryPoint == null)
            {
                throw new InvalidOperationException("Tutorial Gacha Entry Point Not Found");
            }
            // PossessionManager ロード。
            var possessionManager = _checkAndGetPossessionManager(playerId, entryPoint, false);

            // トランザクション
            return ContextContainer.ShardTransaction(tx =>
            {
                return _lot(playerId, entryPoint, false, possessionManager, (playerGachaEntryPoint, lottedContentPossessionParams, error) =>
                {
                    if (error != null) return new LotResponse { Error = error };

                    playerStatus = possessionManager.LoadPlayerStatusWithLock();
                    if (!playerStatus.CanLotTutorialGacha())
                    {
                        throw new InvalidOperationException("Cannot Lot Tutorial Gacha");
                    }
                    playerStatus.TryToProceedTutorialStep(TutorialStep.LotGacha);
                    playerStatus.Save();

                    tx.Commit();

                    return new LotResponse
                    {
                        GachaEntryPoint = entryPoint.ToResponseData(playerGachaEntryPoint, possessionManager),
                        LotPossessionObjects = possessionManager.GetPossessionObjects(lottedContentPossessionParams).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }

        static PossessionManager _checkAndGetPossessionManager(uint playerId, GachaEntryPointEntity entryPoint, bool free)
        {
            // PossessionManager ロード。
            var possessionManager = new PossessionManager(playerId, entryPoint.GetAllPossessionParams());
            possessionManager.Load();
            // 支払い可否チェック（SLAVE）
            if (!free && !possessionManager.CanRemove(entryPoint.GetPaymentPossessionParam()))
            {
                throw new InvalidOperationException(string.Join("\t", "Cannot Remove Possession", playerId, entryPoint.ID));
            }
            return possessionManager;
        }

        static T _lot<T>(uint playerId, GachaEntryPointEntity entryPoint, bool free, PossessionManager possessionManager, Func<PlayerGachaEntryPointEntity, IEnumerable<PossessionParam>, GachaError, T> func)
        {
            // プレイヤーガチャ状態をビルド。
            var playerGachaEntryPoint = PlayerGachaEntryPointEntity.CreateOrReadAndBuild(playerId, entryPoint.ID);
            // 抽選できるかチェック。
            if (!playerGachaEntryPoint.CanLot(entryPoint, free))
            {
                return func(playerGachaEntryPoint, null, new GachaError { CannotLot = true });
            }
            // 抽選実行。
            var lottedContents = playerGachaEntryPoint.Lot(entryPoint, free);
            if (!playerGachaEntryPoint.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Gacha Entry Point", playerId, playerGachaEntryPoint.GachaEntryPointID));
            }
            // 支払いと抽選結果の受け取り。
            var lottedContentPossessionParams = lottedContents.Select(content => content.GetPossessableParam());
            if (!free)
            {
                possessionManager.Remove(entryPoint.GetPaymentPossessionParam());
            }
            possessionManager.Add(lottedContentPossessionParams);

            // クエスト：ガチャページでのガチャ累計実行回数.
            QuestTriggerEntity.CountUpExecutedGachaNum(playerId, entryPoint.GetPaymentPossessionParam());

            return func(playerGachaEntryPoint, lottedContentPossessionParams, null);
        }
    }
}
