using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.GameContent;
using Noroshi.Core.Game.BattleContents;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Player
{
    public class PlayerStatusService
    {
        public static PlayerStatus Get()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var status = PlayerStatusEntity.ReadAndBuild(playerId);
            return status.ToResponseData();
        }

        public static OtherPlayerStatus GetOhter(uint id)
        {
            var status = PlayerStatusEntity.ReadAndBuild(id);
            return status.ToOtherResponseData();
        }

        public static PlayerStatusLevelUpResponse AddExp(uint exp)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var beforePlayerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            var importantContentsUnlocks = new List<ContentsUnlockResponse>();
            var contentsUnlocks = new List<ContentsUnlockResponse>();
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null) {
                throw new NullReferenceException("Player's status data is not found:" + playerId);
            }
            playerStatus.AddExp((ushort)exp);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });

            if (beforePlayerStatus.Level < playerStatus.Level) {
                var releaseLevelData = GameContent.BuildOpenGameContentsByPlayerLevel(beforePlayerStatus.Level, playerStatus.Level);

                releaseLevelData.ToList().ForEach(release =>
                {
                    var tmp = new ContentsUnlockResponse
                    {
                        ContentsID = release.ID,
                        Text = Contents.ContentsCategory[release.ID] + "が解放されました！"
                    };
                    importantContentsUnlocks.Add(tmp);
                });
            }

            var returnData = new PlayerStatusLevelUpResponse
            {
                BeforePlayerStatus = beforePlayerStatus.ToLevelUpResponseData(),
                AfterPlayerStatus = playerStatus.ToLevelUpResponseData(),
                ImportantContentsUnlocks = importantContentsUnlocks.ToArray(),
                ContentsUnlocks = contentsUnlocks.ToArray()
            };

            return returnData;
        }

        public static PlayerStatus AddGold(uint gold)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new NullReferenceException("Player's StatusData is not found:" + playerId);
            }
            playerStatus.ChangeGold(playerStatus.Gold + gold);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus UseGemWithPlayer(uint gem, ushort count, byte type)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus.TotalGem < gem)
            {
                throw new InvalidOperationException();
            }
            playerStatus.UseGem(gem);

            switch (type) {
                case 0:
                    playerStatus.ChangeGold(playerStatus.Gold + count);
                    break;
                case 1:
                    playerStatus.ConsumeStamina(count);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }



        // todo お金関係は仕様ができてから確実に作るように。
        public static PlayerStatus AddChargeGem(uint gem)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.ChangeChargeGem(playerStatus.ChargeGem + (uint)(gem * (playerStatus.PlayerVipLevelBonus.GemBonus + 1.00)));
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus AddFreeGem(uint gem)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.ChangeFreeGem(playerStatus.FreeGem + gem);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus AddVipExp(uint exp)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.AddVipExp(exp);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeAvaterCharacterID(ushort id)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.ChangeAvaterCharacterID(id);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeName(string name)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            playerStatus.ChangeName(name);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeExp(uint exp)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.SetExp(exp);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeLevel(ushort level)
        {
            return ChangeExp(PlayerLevelEntity.GetNecessaryExp(level));
        }

        public static PlayerStatus ChangeStamina(ushort stamina)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.SetStamina(stamina);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeGold(uint gold)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.ChangeGold(gold);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }

        public static PlayerStatus ChangeGem(uint gem)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
            if (playerStatus == null) {
                throw new InvalidOperationException();
            }
            playerStatus.ChangeFreeGem(gem);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var result = playerStatus.Save();
                tx.Commit();
                return result;
            });
            return playerStatus.ToResponseData();
        }
    }
}