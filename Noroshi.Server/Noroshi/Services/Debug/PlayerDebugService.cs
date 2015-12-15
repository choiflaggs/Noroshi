using System;
using Noroshi.Core.WebApi.Response.Debug;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Debug
{
    public class PlayerDebugService
    {
        const string TEMP_UDID = "TEMP-UDID";

        /// <summary>
        /// 対象プレイヤーを（疑似的に）リセットする。
        /// </summary>
        /// <param name="playerId">リセットプレイヤーID</param>
        /// <returns></returns>
        public static PlayerDebugResponse Reset(uint playerId)
        {
            // セッション ID 更新。
            ContextContainer.CommonTransaction(tx =>
            {
                var player = PlayerEntity.ReadAndBuild(playerId, ReadType.Lock);
                player.ChangeToDummyPlayer();
                if (!player.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player", player.ID));
                }
                tx.Commit();
            });
            return new PlayerDebugResponse();
        }

        /// <summary>
        /// プレイヤー情報（ログイン時に利用する端末 ID）を入れ替える。
        /// </summary>
        /// <param name="playerId">入れ替え実行者プレイヤー ID</param>
        /// <param name="targetPlayerId">被入れ替えプレイヤー ID</param>
        /// <returns></returns>
        public static PlayerDebugResponse Swap(uint playerId, uint targetPlayerId)
        {
            return ContextContainer.CommonTransaction(tx =>
            {
                var player = PlayerEntity.ReadAndBuild(playerId, ReadType.Lock);
                var targetPlayer = PlayerEntity.ReadAndBuild(targetPlayerId, ReadType.Lock);
                var udid = player.UDID;
                var targetUdid = targetPlayer.UDID;
                // ユニーク制約に引っかからないように二回に分けて更新（一回目）。
                player.SetUDID(TEMP_UDID);
                targetPlayer.SetUDID(udid);
                if (!player.Save() || !targetPlayer.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player", player.ID, targetPlayer.ID));
                }
                // ユニーク制約に引っかからないように二回に分けて更新（二回目）。
                player.SetUDID(targetUdid);
                if (!player.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player", player.ID));
                }
                tx.Commit();
                return new PlayerDebugResponse();
            });
        }
    }
}
