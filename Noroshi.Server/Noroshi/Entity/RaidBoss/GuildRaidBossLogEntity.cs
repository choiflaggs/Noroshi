using System;
using System.Collections.Generic;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRaidBossLogSchema;

namespace Noroshi.Server.Entity.RaidBoss
{
    /// <summary>
    /// ギルドレイドボスに対するログを表現するクラス。
    /// </summary>
    public class GuildRaidBossLogEntity : AbstractDaoWrapperEntity<GuildRaidBossLogEntity, GuildRaidBossLogDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// ギルドレイドボスに対するログを全て取得。
        /// 内部的に新しいもの順にソートされている。
        /// </summary>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <param name="rowCount">最大取得件数</param>
        /// <returns></returns>
        public static IEnumerable<GuildRaidBossLogEntity> ReadAndBuildByGuildRaidBossIDOrderByCreatedAtDesc(uint guildRaidBossId, uint guildRaidBossCreatedAt, ushort rowCount)
        {
            return _instantiate((new GuildRaidBossLogDao()).ReadByGuildRaidBossIDOrderByCreatedAtDesc(guildRaidBossId, guildRaidBossCreatedAt, rowCount));
        }
        /// <summary>
        /// ログを新規作成。
        /// </summary>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <param name="playerId">バトル実行プレイヤーID</param>
        /// <param name="damage">与えたダメージ</param>
        /// <returns></returns>
        public static GuildRaidBossLogEntity Create(uint guildRaidBossId, uint guildRaidBossCreatedAt, uint playerId, uint damage)
        {
            return _instantiate((new GuildRaidBossLogDao()).Create(guildRaidBossId, guildRaidBossCreatedAt, playerId, damage));
        }


        public uint PlayerID => _record.PlayerID;
        public uint Damage => _record.Damage;
        public uint CreatedAt => _record.CreatedAt;

        public Core.WebApi.Response.RaidBoss.RaidBossLog ToResponseData(PlayerStatusEntity playerStatus)
        {
            if (playerStatus.PlayerID != PlayerID) throw new InvalidOperationException();
            return new Core.WebApi.Response.RaidBoss.RaidBossLog
            {
                Player = playerStatus.ToOtherResponseData(),
                Damage = Damage,
                CreatedAt = CreatedAt,
            };
        }
    }
}
