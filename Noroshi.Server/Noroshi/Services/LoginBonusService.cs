using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.LoginBonus;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class LoginBonusService
    {
        /// <summary>
        /// 指定プレイヤーのログインボーナス一覧を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.LoginBonus.ListResponse List(uint playerId)
        {
            // 開いているログインボーナスは全てビルド。
            var loginBonuses = LoginBonusEntity.ReadAndBuildOpenLoginBonusByCurrentTime(ContextContainer.GetContext().TimeHandler.UnixTime);
            // 仮に開いているログインボーナスが存在しなければ空配列で即レスポンスを返す。
            if (loginBonuses.Count() == 0)
            {
                /* レスポンス */

                return new Core.WebApi.Response.LoginBonus.ListResponse
                {
                    LoginBonuses = new Core.WebApi.Response.LoginBonus.LoginBonus[0],
                };
            }
            // 何回か利用するのでログインボーナスIDへの変換結果を変数に入れておく。
            var loginBonusIds = loginBonuses.Select(loginBonus => loginBonus.ID);

            // プレイヤーログインボーナス状況をビルドし、ログインボーナスID => プレイヤーログインボーナス状況のマッピングを作る。
            var loginBonusIdToPlayerLoginBonus = PlayerLoginBonusEntity.ReadAndBuildByPlayerIDAndLoginBonusIDs(playerId, loginBonusIds)
                .ToDictionary(plb => plb.LoginBonusID);

            // カウントアップ対象のログインボーナスIDを抽出。
            var countUpPlayerLoginBonusIds = loginBonusIds.Where(loginBonusId => !loginBonusIdToPlayerLoginBonus.ContainsKey(loginBonusId) || loginBonusIdToPlayerLoginBonus[loginBonusId].CanCountUp());
            // カウントアップ対象があれば、カウントアップ処理を実行し、
            if (countUpPlayerLoginBonusIds.Count() > 0)
            {
                /* トランザクション */

                var playerLoginBonuses = ContextContainer.NoroshiTransaction(tx => _countUpLoginNum(playerId, countUpPlayerLoginBonusIds, tx));

                // 既に変数に入っているプレイヤーログインボーナス状況を更新。
                foreach (var playerLoginBonus in playerLoginBonuses)
                {
                    if (loginBonusIdToPlayerLoginBonus.ContainsKey(playerLoginBonus.LoginBonusID))
                    {
                        loginBonusIdToPlayerLoginBonus[playerLoginBonus.LoginBonusID] = playerLoginBonus;
                    }
                    else
                    {
                        loginBonusIdToPlayerLoginBonus.Add(playerLoginBonus.LoginBonusID, playerLoginBonus);
                    }
                }
            }

            // 報酬をロード。
            var possessionParams = loginBonuses.SelectMany(loginBonus => loginBonus.GetPossessionParams());
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            var vipLevel = PlayerStatusEntity.ReadAndBuild(playerId).VipLevel;
            /* レスポンス */

            return new Core.WebApi.Response.LoginBonus.ListResponse
            {
                LoginBonuses = loginBonuses.Select(loginBonus =>
                {
                    var playerLoginBonus = loginBonusIdToPlayerLoginBonus.ContainsKey(loginBonus.ID) ? loginBonusIdToPlayerLoginBonus[loginBonus.ID] : null;
                    return loginBonus.ToResponseData(playerLoginBonus, possessionManager, vipLevel);
                }).ToArray(),
            };
        }
        static IEnumerable<PlayerLoginBonusEntity> _countUpLoginNum(uint playerId, IEnumerable<uint> loginBonusIds, TransactionContainer transaction)
        {
            var playerLoginBonuses = loginBonusIds.Select(loginBonusId => PlayerLoginBonusEntity.CountUp(playerId, loginBonusId));
            transaction.Commit();
            return playerLoginBonuses;
        }

        /// <summary>
        /// 指定プレイヤーに指定ログインボーナス報酬を受け取らせる。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="loginBonusRewardId">対象ログインボーナス報酬ID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.LoginBonus.ReceiveRewardResponse ReceiveReward(uint playerId, uint loginBonusId, byte threshold)
        {

            // 対象ログインボーナスをビルド。
            var loginBonus = LoginBonusEntity.ReadAndBuild(loginBonusId);
            // データ構造上、必ずログインボーナスは存在する。
            if (loginBonus == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Login Bonus Not Found", loginBonusId));
            }

            // 対象プレイヤーログインボーナス状況をビルド。
            var playerLoginBonus = PlayerLoginBonusEntity.ReadAndBuild(playerId, loginBonus.ID);
            // 遷移上、プレイヤーログインボーナス状況が存在しないことはあり得ない。
            if (playerLoginBonus == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Player Login Bonus Not Found", playerId, loginBonus.ID));
            }
            
            // 対象ログインボーナス報酬をビルド。
            var loginBonusRewards = loginBonus.GetRewardsByThreshold(threshold);
            var loginBonusRewardIds = loginBonusRewards.Select(reward => reward.ID);
            // 遷移上、存在しないログインボーナス報酬が指定されることはない。
            if (loginBonusRewards.Count() == 0)
            {
                throw new InvalidOperationException(string.Join("\t", "Login Bonus Rewards Not Found", loginBonusRewardIds));
            }
            // 遷移上、報酬を受け取れないことはあり得ない。
            if (!playerLoginBonus.CanReceiveReward(threshold))
            {
                throw new InvalidOperationException(string.Join("\t", "Cannot Receive Login Bonus Rewards", loginBonusRewardIds));
            }

            // 報酬付与準備。
            PlayerStatusEntity playerStatus = null;
            ushort vipLevel = 0;
            IEnumerable<PossessionParam> rewardPossessionParams = null;
            PossessionManager possessionManager = null;

            /* トランザクション */
            playerLoginBonus = ContextContainer.NoroshiTransaction(tx =>
            {
                // ロックを掛けて取得。
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                vipLevel = PlayerStatusEntity.ReadAndBuild(playerId).VipLevel;
                rewardPossessionParams = loginBonusRewards.Select(reward => reward.GetPossessionParam(LoginBonusRewardEntity.HasVipBonus(reward.Threshold, vipLevel, loginBonus.Category)));
                possessionManager = new PossessionManager(playerId, rewardPossessionParams);
                
                // 報酬付与以外のログインボーナス固有更新処理実行。
                var playerLoginBonusWithLock = _receiveDailyReward(playerId, loginBonusId, threshold);
                // 報酬付与。
                possessionManager.Add(rewardPossessionParams);

                tx.Commit();
                return playerLoginBonusWithLock;
            });

            /* レスポンス */

            return new Core.WebApi.Response.LoginBonus.ReceiveRewardResponse
            {
                VipLevel = vipLevel,
                HasVipReward = LoginBonusRewardEntity.HasVipBonus(threshold, vipLevel, loginBonus.Category),
                RewardPossessionObjects = possessionManager.GetPossessionObjects(rewardPossessionParams).Select(po => po.ToResponseData()).ToArray(),
            };
        }
        static PlayerLoginBonusEntity _receiveDailyReward(uint playerId, uint loginBonusId, byte threshold)
        {
            // 対象プレイヤーログインボーナス状況をビルド。更新に利用するのでロック付き。
            var playerLoginBonus = PlayerLoginBonusEntity.ReadAndBuild(playerId, loginBonusId, ReadType.Lock);
            // 遷移上、プレイヤーログインボーナス状況が存在しないことはあり得ない。
            if (playerLoginBonus == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Player Login Bonus Not Found", playerId, loginBonusId));
            }
            // 遷移上、報酬を受け取れないことはあり得ない。
            if (!playerLoginBonus.CanReceiveReward(threshold))
            {
                throw new InvalidOperationException(string.Join("\t", "Login Bonus Not Found", loginBonusId));
            }

            // 報酬受け取り処理（Possession 系の報酬付与は別途）。
            playerLoginBonus.ReceiveReward(threshold);

            return playerLoginBonus;
        }
    }
}
