using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Core.Game.Arena;
using Noroshi.Core.WebApi.Response.Arena;
using Noroshi.Core.WebApi.Response.Players;
using Noroshi.Server.Daos.Rdb;

using CorePlayer = Noroshi.Core.Game.Player;

namespace Noroshi.Server.Services.Player
{
    public class PlayerArenaService
    {
        public static Core.WebApi.Response.Arena.PlayerArena GetPlayerArena()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.CreateOrReadAndBuild(playerId);
            ContextContainer.NoroshiTransaction(tx =>
            {
                playerArena.ResetTimeCheck();
                var result = playerArena.Save();
                tx.Commit();
                return result;
            });
            return playerArena.ToResponseData();
        }

        public static PlayerArenaOtherResponse[] GetBattleCandidates()
        {
            // プレイヤー情報取得　1度でもアリーナをリザルトまで行った事が無いユーザはレコードが無い.
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.BuildOrReadAndBuild(playerId);
            var playerRank = playerArena.Rank != 0 ? playerArena.Rank : PlayerArenaEntity.GetArenaLastRank;

            // プレイヤーが100位内とそれ以下で検索ロジックを切り替え.
            var playerArenaEntities = new List<PlayerArenaEntity>();
            if ( playerRank < Constant.ARENA_RANKING_SEARCH_RANGE_NUM)
            {
                // 1位から100位のランカーから取得する　3件取得できなければ50位ずつ下位を探す.
                playerArenaEntities = _getPlayerArena(playerId, 1, Constant.ARENA_RANKING_SEARCH_EXTEND_RANGE_NUM, PlayerArenaEntity.ReadByRankOverAndRowCountOtherSsearcher);
            }
            else
            {
                // 自分よりrankが小さい100人を探す     3件取得できなければ50件ずつ上位を探す.                                                                                                                                         
                playerArenaEntities = _getPlayerArena(playerId, playerRank, Constant.ARENA_RANKING_SEARCH_EXTEND_RANGE_NUM * -1, PlayerArenaEntity.ReadByRankUnderAndRowCountOtherSsearcher);
            }


            if (playerArenaEntities.Count < Constant.ARENA_LISTING_PLAYER_NUM)
                // 戦えるプレイヤーが3人以下だった.
                return playerArenaEntities.Select(entity => entity.ToOtherResponseData()).ToArray();
            else
                // ランダムで3人抽出.
                return playerArenaEntities.OrderBy(i => Guid.NewGuid()).Take(Constant.ARENA_LISTING_PLAYER_NUM).Select( entity => entity.ToOtherResponseData()).ToArray();
        }

        static List<T> _getPlayerArena<T>(uint playerId, uint rank, int addRank, Func<uint, uint, uint,IEnumerable<T>> ReadPlayerArena)
        where T : PlayerArenaEntity
        {
            uint limitLow = PlayerArenaEntity.GetArenaLastRank;
            uint rowLimit = Constant.ARENA_RANKING_SEARCH_RANGE_NUM;
            var SearchCountLast = Constant.ARENA_LISTING_SEARCH_LIMIT_NUM;

            var playerArenaRecord = new List<T>();
            while ( 0 < SearchCountLast && playerArenaRecord.Count() < Constant.ARENA_LISTING_PLAYER_NUM && ( 0 < rank && rank <= limitLow))
            {
                // BattleStartedAtが120秒経過しているユーザを取得.
                var playerArenaEntitys = ReadPlayerArena(rank, rowLimit, playerId).Where(entity => entity.BattleStartedAt + Constant.ARENA_LISTING_NEXT_BATTLE_SPAN.TotalSeconds < ContextContainer.GetContext().TimeHandler.UnixTime);
                playerArenaRecord.AddRange(playerArenaEntitys.ToList());

                // 次周準備.
                rowLimit = (uint)(Constant.ARENA_RANKING_SEARCH_EXTEND_RANGE_NUM - playerArenaEntitys.Count());
                rank    = (uint)(rank + addRank); // uint + int.
                SearchCountLast--;
            }

            return playerArenaRecord;
        }


        public static Core.WebApi.Response.Arena.PlayerArena ChangeDeck(uint[] characterIds)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.ReadAndBuild(playerId);
            ContextContainer.NoroshiTransaction(tx =>
            {
                playerArena.ChangeDeck(characterIds);
                var result = playerArena.Save();
                tx.Commit();
                return result;
            });
            return playerArena.ToResponseData();
        }

        public static Core.WebApi.Response.Arena.PlayerArena ChangeRank(uint otherPlayerId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.ReadAndBuild(playerId);
            var queryArenaOtherPlayerData = PlayerArenaEntity.ReadAndBuild(otherPlayerId);
            ContextContainer.NoroshiTransaction(tx =>
            {
                var tmpRank = playerArena.Rank;
                playerArena.ChangeRank(queryArenaOtherPlayerData.Rank);
                queryArenaOtherPlayerData.ChangeRank(tmpRank);
                var result = playerArena.Save() || queryArenaOtherPlayerData.Save();
                tx.Commit();
                return result;
            });
            return playerArena.ToResponseData();
        }

        public static ArenaServiceResponse ResetCoolTime()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.ReadAndBuild(playerId);
            if(playerArena == null) throw new InvalidOperationException(); // アリーナで1度も対戦していないユーザにはレコードが無い.

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            var paymentCalculator = new CorePlayer.RepeatablePaymentCalculator(Constant.GEM_RESET_COOLTIME_NUM);
            uint useResetGemPoint = paymentCalculator.GetPaymentNum((ushort)playerArena.CoolTimeResetNum);
            if (playerStatus.TotalGem < useResetGemPoint)
            {
                // 状態: エラー ジェムの所持数が足りない.
                throw new InvalidOperationException(string.Join("\t", "Arena ResetCoolTime ShortfallGem", playerId, playerStatus.TotalGem));
            }
            if (playerArena.CoolTimeAt == 0 || playerArena.CoolTimeAt < ContextContainer.GetContext().TimeHandler.UnixTime)
            {
                // 状態:クリアする必要がないのにジェムを使うところだった.
                return new ArenaServiceResponse() { Error = new PlayerError() { NoNeedResetCoolTime = true } };
            }

            // ジェムを消費してクールタイムをリセットして直ぐに戦えるようにする.
            ContextContainer.NoroshiTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerArena = PlayerArenaEntity.ReadAndBuild(playerId, ReadType.Lock);

                playerStatus.UseGem(useResetGemPoint);
                playerArena.ResetCoolTime();

                if (!playerArena.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save PlayerArena", playerId));
                }
                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save PlayerStatus", playerId));
                }

                tx.Commit();
                return true;
            });
            return playerArena.ToServiceResponseData(playerStatus);
        }


        public static ArenaServiceResponse ResetPlayNum()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            var playerArena = PlayerArenaEntity.ReadAndBuild(playerId);
            if (playerArena == null) throw new InvalidOperationException(); // アリーナで1度も対戦していないユーザにはレコードが無い.

            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            var paymentCalculator = new CorePlayer.RepeatablePaymentCalculator(Constant.GEM_RESET_PLAY_NUM);
            uint useResetGemPoint = paymentCalculator.GetPaymentNum((ushort)playerArena.PlayResetNum);
            if (playerStatus.TotalGem < useResetGemPoint)
            {
                // 状態: エラー ジェムの所持数が足りない.
                throw new InvalidOperationException(string.Join("\t", "Arena ResetPlayNum ShortfallGem", playerId, playerStatus.TotalGem));
            }
            if (playerArena.PlayNum == 0 || Constant.ARENA_DAILY_PLAY_LIMIT_NUM < playerArena.PlayNum)
            {
                // 状態:クリアする必要がないのにジェムを使うところだった.
                return new ArenaServiceResponse() { Error = new PlayerError() { NoNeedResetPlayNum = true } };
            }

            // ジェムを消費してデイリー戦闘回数制限をクリアする.
            ContextContainer.NoroshiTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerArena = PlayerArenaEntity.ReadAndBuild(playerId, ReadType.Lock);

                playerStatus.UseGem(useResetGemPoint);
                playerArena.ResetPlayNum();

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save PlayerStatus", playerId));
                }

                if (!playerArena.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save PlayerArena", playerId));
                }
                tx.Commit();
                return true;
            });
            return playerArena.ToServiceResponseData(playerStatus);
        }


    }
}