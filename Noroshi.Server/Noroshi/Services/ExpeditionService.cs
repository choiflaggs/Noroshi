using System;
using System.Linq;
using Noroshi.Core.WebApi.Response.Expedition;
using Noroshi.Core.Game.GameContent;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Expedition;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class ExpeditionService
    {
        /// <summary>
        /// 冒険関連情報を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static GetResponse Get(uint playerId)
        {
            // 開放判定。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (!GameContent.IsOpen(GameContentID.Expedition, playerStatus.Level))
            {
                throw new InvalidOperationException(string.Join("\t", "Expedition Not Open", playerStatus.PlayerID, playerStatus.Level));
            }
            // プレイヤー冒険状態をビルド。
            var playerExpedition = PlayerExpeditionEntity.ReadOrDefaultAndBuild(playerId);
            var session = PlayerExpeditionSessionEntity.ReadOrDefaultAndBuild(playerId);
            // 現冒険を取得。
            var currentExpedition = ExpeditionEntity.ReadAndBuildCurrentByClearLevel(playerExpedition.ClearLevel);
            if (currentExpedition == null)
            {
                throw new SystemException(string.Join("\t", "Current Expedition Not Found", playerId));
            }
            // 冒険中であれば報酬をロード。
            var possessionManager = session.IsActive ? new PossessionManager(playerId, session.GetAllRewards(playerStatus.PlayerVipLevelBonus)) : null;
            if (possessionManager != null) possessionManager.Load();

            return new GetResponse()
            {
                CurrentExpedition = currentExpedition.ToResponseData(),
                PlayerExpedition = playerExpedition.ToResponseData(playerStatus.VipLevel, session, possessionManager, playerStatus.PlayerVipLevelBonus),
            };
        }

        /// <summary>
        /// 冒険を開始する。
        /// </summary>
        /// <param name="playerId">冒険開始プレイヤーID</param>
        /// <returns></returns>
        public static StartResponse Start(uint playerId)
        {
            // 開放判定。
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (!GameContent.IsOpen(GameContentID.Expedition, playerStatus.Level))
            {
                throw new InvalidOperationException(string.Join("\t", "Expedition Not Open", playerStatus.PlayerID, playerStatus.Level));
            }

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックを掛けてプレイヤー冒険状態をビルド。
                var playerExpedition = PlayerExpeditionEntity.CreateOrRead(playerId);
                var session = PlayerExpeditionSessionEntity.CreateOrRead(playerId);

                // 現冒険を取得。
                var expedition = ExpeditionEntity.ReadAndBuildCurrentByClearLevel(playerExpedition.ClearLevel);
                if (expedition == null)
                {
                    throw new SystemException(string.Join("\t", "Current Expedition Not Found", playerId));
                }
                // 開始可否チェック。
                if (!playerExpedition.CanStart(session, expedition))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Start", playerExpedition.PlayerID));
                }
                // トランザクション内でやや重い処理だが、ロックの関係上致し方ない。
                var stageSessionData = playerExpedition.MakeStageData(playerId, expedition);
                // 開始処理実行。
                playerExpedition.Start(session, expedition, stageSessionData);
                if (!playerExpedition.Save() || !session.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save", playerExpedition.PlayerID));
                }

                tx.Commit();

                // 報酬ロード。
                var possessionManager = new PossessionManager(playerId, session.GetAllRewards(playerStatus.PlayerVipLevelBonus));
                possessionManager.Load();

                return new StartResponse()
                {
                    PlayerExpedition = playerExpedition.ToResponseData(playerStatus.VipLevel, session, possessionManager, playerStatus.PlayerVipLevelBonus),
                };
            });
        }

        /// <summary>
        /// 獲得資格のある報酬を獲得する。
        /// </summary>
        /// <param name="playerId">報酬獲得プレイヤーID</param>
        /// <returns></returns>
        public static ReceiveRewardResponse ReceiveReward(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックを掛けてプレイヤー冒険状態をビルド。
                var playerExpedition = PlayerExpeditionEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (playerExpedition == null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Player Expedition Not Found", playerId));
                }
                var session = PlayerExpeditionSessionEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (session == null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Player Expedition Session Not Found", playerId));
                }

                // 報酬受け取り可否チェック。
                if (!playerExpedition.CanReceiveReward(session))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Receive Reward", playerExpedition.PlayerID));
                }
                // 報酬を変数に格納した上で受け取り状態へ更新。
                var rewards = session.GetCurrentRewards(playerStatus.PlayerVipLevelBonus);
                playerExpedition.ReceiveReward(session);
                if (!playerExpedition.Save() || !session.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save", playerExpedition.PlayerID));
                }
                // 報酬付与。
                var possessionManager = new PossessionManager(playerId, session.GetAllRewards(playerStatus.PlayerVipLevelBonus));
                possessionManager.Load();
                possessionManager.Add(rewards);

                tx.Commit();

                return new ReceiveRewardResponse()
                {
                    PlayerExpedition = playerExpedition.ToResponseData(playerStatus.VipLevel, session, possessionManager, playerStatus.PlayerVipLevelBonus),
                    Rewards = possessionManager.GetPossessionObjects(rewards).Select(po => po.ToResponseData()).ToArray(),
                };
            });
        }

        /// <summary>
        /// 現在プレイ中の冒険をリセットする。
        /// </summary>
        /// <param name="playerId">リセットするプレイヤーID</param>
        /// <returns></returns>
        public static ResetResponse Reset(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            return ContextContainer.ShardTransaction(tx =>
            {
                // ロックを掛けてプレイヤー冒険状態をビルド。
                var playerExpedition = PlayerExpeditionEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (playerExpedition == null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Player Expedition Not Found", playerId));
                }
                var session = PlayerExpeditionSessionEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (session == null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Player Expedition Session Not Found", playerId));
                }

                // リセット可否チェック。
                if (!playerExpedition.CanReset(session, playerStatus.VipLevel))
                {
                    return new ResetResponse { Error = new ExpeditionError { CannotReset = true } };
                }
                // リセット実行。
                playerExpedition.Reset(session);
                if (!playerExpedition.Save() || !session.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save", playerExpedition.PlayerID));
                }

                tx.Commit();

                return new ResetResponse()
                {
                    PlayerExpedition = playerExpedition.ToResponseData(playerStatus.VipLevel, session, null, playerStatus.PlayerVipLevelBonus),
                };
            });
        }
    }
}
