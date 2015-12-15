using System;
using System.Linq;
using Noroshi.Core.Game.Player;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Core.WebApi.Response.Players;
using CharacterConstant = Noroshi.Core.Game.Character.Constant;

namespace Noroshi.Server.Services.Player
{
    public class PlayerService
    {
        public static SessionData Login(string udid)
        {
            if (string.IsNullOrEmpty(udid))
            {
                throw new InvalidOperationException("No UDID");
            }
            // セッション ID 生成。
            var sessionId = Guid.NewGuid().ToString("N");

            var player = PlayerEntity.ReadAndBuildByUDID(udid);
            if (player == null)
            {
                // シャード決定。
                var shardId = (new ShardDispatcherEntity()).GetShardID();
                ContextContainer.GetWebContext().SetShardID(shardId);   // TODO： 後で消す！
                // 初期付与キャラクター
                var firstCharacters = CharacterEntity.ReadAndBuildMulti(new uint[] { CharacterConstant.FIRST_CHARACTER_ID });
                // プレイヤー作成トランザクション。
                ContextContainer.CommonAndShardTransaction(shardId, tx =>
                {
                    player = PlayerEntity.Create(udid, sessionId, shardId);
                    // UDID もしくはセッション ID が重複したら例外投げてしまう。
                    // UDID が重複する場合は SLAVE チェックすり抜けであり得るが頻度を考えると問題ないし、
                    // セッション ID 重複は確率的にほぼないはず。
                    if (player == null)
                    {
                        throw new SystemException(string.Join("\t", "Duplicate Player", udid, sessionId));
                    }
                    // Shard ID を割り振ったばかりなので、外から Shard を指定しつつ PlayerStatus を作成。
                    var playerStatus = PlayerStatusEntity.Create(player.ID, (uint)Language.Japanese, player.ShardID);
                    if (playerStatus == null)
                    {
                        throw new SystemException(string.Join("\t", "Duplicate Player Status", player.ID, udid, sessionId, shardId));
                    }
                    // Shard ID を割り振ったばかりなので、外から Shard を指定しつつ初期 Character 付与。
                    var playerCharacters = firstCharacters.Select(character => PlayerCharacterEntity.Create(player.ID, character.ID, character.InitialEvolutionLevel, player.ShardID));
                    if (playerCharacters.Count() != firstCharacters.Count())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Create Player Character", player.ID));
                    }
                    // ログ。
                    tx.AddAfterCommitAction(() => ContextContainer.GetContext().Logger.Info("Register Player", player.ID, udid, sessionId, shardId));

                    tx.Commit();
                });
            }
            else
            {
                // セッション ID 更新。
                ContextContainer.CommonTransaction(tx =>
                {
                    player = PlayerEntity.ReadAndBuild(player.ID, ReadType.Lock);
                    player.SetSessionID(sessionId);
                    if (!player.Save())
                    {
                        throw new SystemException(string.Join("\t", "Duplicate Session ID", player.ID, udid, sessionId));
                    }
                    tx.Commit();
                });
            }
            return new SessionData
            {
                SessionID = player.SessionID,
            };
        }

        public static PlayerServiceResponse RecoverStamina(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null)
            {
                // 状態:エラー プレイヤーデータが無い.
                throw new InvalidOperationException (string.Join("\t", "Player Status Not Found", playerId));
            }
            if (playerStatus.PlayerVipLevelBonus.MaxStaminaRecoveryNum <= playerStatus.StaminaRecoveryNum)
            {
                // 状態:エラー 回復上限に達している
                throw new InvalidOperationException($"Player Stamina Recovery Count Over PlayerID: {playerId} VipLevel {playerStatus.VipLevel} RecoveryCount {playerStatus.StaminaRecoveryNum}");
            }
            var paymentCalculator = new RepeatablePaymentCalculator(Constant.RECOVER_GEM_POINT);
            uint useRecoverGemPoint = paymentCalculator.GetPaymentNum((ushort)playerStatus.StaminaRecoveryNum);
            if (playerStatus.TotalGem < useRecoverGemPoint)
            {
                // 状態:エラー ジェムの所持数が足りない.
                throw new InvalidOperationException (string.Format("Player RecoverStamina ShortfallGem PlayerID : {0} NeedGem : {1}  HaveGem : {2} ", playerId, useRecoverGemPoint, playerStatus.TotalGem));
            }
            if (playerStatus.Stamina >= playerStatus.MaxStamina)
            {
                // 状態:エラー スタミナを回復する必要が無い.
                return new PlayerServiceResponse()
                {
                    Error        = new PlayerError() { NoNeedRecoverStamina = true },
                    PlayerStatus = playerStatus.ToResponseData()
                };
            }

            // ジェムを消費してスタミナ回復.
            ContextContainer.ShardTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerStatus.SetStamina(playerStatus.MaxStamina);
                playerStatus.UseGem(useRecoverGemPoint);
                playerStatus.IncrementStaminaRecoveryNum();

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Cannot Update", playerStatus.PlayerID));
                }
                tx.Commit();
            });

            return new PlayerServiceResponse()
            {
                PlayerStatus = playerStatus.ToResponseData()
            };

        }

        public static PlayerServiceResponse RecoverBP(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            if (playerStatus == null)
            {
                // 状態:エラー プレイヤーデータが無い.
                throw new InvalidOperationException (string.Join("\t", "Player Status Not Found", playerId));
            }
            //var paymentCalculator = new RepeatablePaymentCalculator(Constant.RECOVER_GEM_POINT);
            //uint useRecoverGemPoint = paymentCalculator.GetPaymentNum((ushort)playerStatus.BPRecaveryNum);
            uint useRecoverGemPoint = Constant.RECOVER_GEM_POINT;
            if (playerStatus.TotalGem < useRecoverGemPoint)
            {
                // 状態: エラー ジェムの所持数が足りない.
                throw new InvalidOperationException (string.Format("Player RecoverBP ShortfallGem PlayerID : {0} NeedGem : {1}  HaveGem : {2} ", playerId, useRecoverGemPoint, playerStatus.TotalGem));
            }
            if (playerStatus.BP >= playerStatus.MaxBP)
            {
                // 状態:エラー BPを回復する必要が無い.
                return new PlayerServiceResponse()
                {
                    Error = new PlayerError() { NoNeedRecoverBP = true },
                    PlayerStatus = playerStatus.ToResponseData()
                };
            }

            // ジェムを消費してBP回復.
            ContextContainer.ShardTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerStatus.SetBP(playerStatus.MaxBP);
                playerStatus.UseGem(useRecoverGemPoint);
                playerStatus.IncrementBPRecaveryNum();

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Cannot Update", playerStatus.PlayerID));
                }
                tx.Commit();
            });

            return new PlayerServiceResponse()
            {
                PlayerStatus = playerStatus.ToResponseData()
            };
        }

        public static PlayerServiceResponse RecoverGold(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            if (playerStatus == null)
            {
                // 状態:エラー プレイヤーデータが無い.
                throw new InvalidOperationException(string.Join("\t", "Player Status Not Found", playerId));
            }
            var paymentCalculator = new RepeatablePaymentCalculator(Constant.RECOVER_GEM_TO_GOLD_POINT);
            uint useRecoverGemPoint = paymentCalculator.GetPaymentNum((ushort)playerStatus.GoldRecaveryNum);
            if (playerStatus.TotalGem < useRecoverGemPoint)
            {
                // 状態: エラー ジェムの所持数が足りない.
                throw new InvalidOperationException($"Player RecoverBP ShortfallGem PlayerID : {playerId} NeedGem : {useRecoverGemPoint}  HaveGem : {playerStatus.TotalGem} ");
            }
            if (playerStatus.Gold >= Constant.MAX_GOLD)
            {
                // 状態:エラー お金を増やせない
                return new PlayerServiceResponse
                {
                    Error = new PlayerError { NoNeedRecoverGold = true },
                    PlayerStatus = playerStatus.ToResponseData()
                };
            }

            // ジェムを消費してBP回復
            ContextContainer.ShardTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerStatus.ChangeGold(playerStatus.Gold + Constant.CONVERT_GEM_TO_GOLD_NUM);
                playerStatus.UseGem(useRecoverGemPoint);
                playerStatus.IncrementGoldRecoveryNum();

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Cannot Update", playerStatus.PlayerID));
                }
                tx.Commit();
            });

            return new PlayerServiceResponse
            {
                PlayerStatus = playerStatus.ToResponseData()
            };

        }

        public static PlayerServiceResponse RecoverActionLevelPoint(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null)
            {
                // 状態:エラー プレイヤーデータが無い.
                throw new InvalidOperationException(string.Join("\t", "Player Status Not Found", playerId));
            }
            var paymentCalculator = new RepeatablePaymentCalculator(Constant.RECOVER_GEM_TO_ACTION_LEVEL_POINT);
            uint useRecoverGemPoint = paymentCalculator.GetPaymentNum((ushort)playerStatus.ActionLevelPointRecoveryNum);
            if (playerStatus.TotalGem < useRecoverGemPoint)
            {
                // 状態:エラー ジェムの所持数が足りない.
                throw new InvalidOperationException(string.Format("Player RecoverActionLevelPoint ShortfallGem PlayerID : {0} NeedGem : {1}  HaveGem : {2} ", playerId, useRecoverGemPoint, playerStatus.TotalGem));
            }
            if (playerStatus.ActionLevelPoint >= playerStatus.MaxActionLevelPoint)
            {
                // 状態:エラー スタミナを回復する必要が無い.
                return new PlayerServiceResponse()
                {
                    Error = new PlayerError() { NoNeedRecoverActionLevelPoint = true },
                    PlayerStatus = playerStatus.ToResponseData()
                };
            }

            // ジェムを消費してスタミナ回復.
            ContextContainer.ShardTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerStatus.SetActionLevelPoint(playerStatus.MaxActionLevelPoint);
                playerStatus.UseGem(useRecoverGemPoint);
                playerStatus.IncrementActionLevelPointRecoveryNum();

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Cannot Update", playerStatus.PlayerID));
                }
                tx.Commit();
            });

            return new PlayerServiceResponse()
            {
                PlayerStatus = playerStatus.ToResponseData()
            };
        }
    }
}
