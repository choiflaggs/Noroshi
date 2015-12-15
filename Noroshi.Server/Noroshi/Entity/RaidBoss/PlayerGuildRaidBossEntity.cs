using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerGuildRaidBossSchema;

namespace Noroshi.Server.Entity.RaidBoss
{
    /// <summary>
    /// プレイヤーとギルドレイドボスとの関係を表現するクラス。
    /// </summary>
    public class PlayerGuildRaidBossEntity : AbstractDaoWrapperEntity<PlayerGuildRaidBossEntity, PlayerGuildRaidBossDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルドメソッド。
        /// 対応レコードはパーティションを切る都合上、対象ギルドレイドボス出現日時も含めて PK であることに注意。、
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerGuildRaidBossEntity ReadAndBuild(uint playerId, uint guildRaidBossId, uint guildRaidBossCreatedAt, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId, GuildRaidBossID = guildRaidBossId, GuildRaidBossCreatedAt = guildRaidBossCreatedAt }, readType);
        }
        /// <summary>
        /// 複数ビルドメソッド。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="guildRaidBosses">対象ギルドレイドボス</param>
        /// <returns></returns>
        public static IEnumerable<PlayerGuildRaidBossEntity> ReadAndBuildByPlayerIDAndGuildRaidBosses(uint playerId,  IEnumerable<GuildRaidBossEntity> guildRaidBosses)
        {
            return ReadAndBuildMulti(guildRaidBosses.Select(grb => new Schema.PrimaryKey { PlayerID = playerId, GuildRaidBossID = grb.ID, GuildRaidBossCreatedAt = grb.CreatedAt }));
        }

        /// <summary>
        /// 該当ギルドレイドボスに対して付与ダメージが多い順にビルドする。
        /// </summary>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <param name="rowCount">最大取得件数</param>
        /// <returns></returns>
        public static IEnumerable<PlayerGuildRaidBossEntity> ReadAndBuildByGuildRaidBossIDAndRaidBossCreatedAtOrderByDamageDesc(uint guildRaidBossId, uint guildRaidBossCreatedAt, ushort rowCount)
        {
            return _instantiate((new PlayerGuildRaidBossDao()).ReadByGuildRaidBossIDAndRaidBossCreatedAtOrderByDamageDesc(guildRaidBossId, guildRaidBossCreatedAt, rowCount));
        }
        /// <summary>
        /// バトル状態のものを古いもの順にビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerGuildRaidBossEntity> ReadAndBuildBattleStateByPlayerIDOrderByGuildRaidBossCreatedAtAsc(uint playerId)
        {
            var recordLifetime = Constant.GUILD_RAID_BOSS_RECORD_LIFE_TIME;
            return _instantiate((new PlayerGuildRaidBossDao()).ReadByPlayerIDAndStateOrderByGuildRaidBossCreatedAtAsc(playerId, PlayerGuildRaidBossState.Battle, (uint)recordLifetime.TotalSeconds, Constant.MAX_SHOWABLE_UNRECEIVED_GUILD_RAID_BOSS_NUM));
        }
        /// <summary>
        /// レコード作成を試行し、失敗したらロックをかけて参照した上でビルドする。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <returns></returns>
        public static PlayerGuildRaidBossEntity CreateOrReadAndBuild(uint playerId, uint guildRaidBossId, uint guildRaidBossCreatedAt)
        {
            return _instantiate((new PlayerGuildRaidBossDao()).CreateOrReadAndBuild(playerId, guildRaidBossId, guildRaidBossCreatedAt));
        }
        /// <summary>
        /// プレイヤーがギルドレイドボスに与えたダメージを記録（加算）する。
        /// 対応レコードはパーティションを切る都合上、対象ギルドレイドボス出現日時も含めて PK であることに注意。、
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="guildRaidBossId">対象ギルドレイドボスID</param>
        /// <param name="guildRaidBossCreatedAt">対象ギルドレイドボス出現日時</param>
        /// <param name="damage">与えたダメージ</param>
        public static void AddDamage(uint playerId, uint guildRaidBossId, uint guildRaidBossCreatedAt, uint damage)
        {
            var entity = CreateOrReadAndBuild(playerId, guildRaidBossId, guildRaidBossCreatedAt);
            entity._addDamage(damage);
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Cannot Update", playerId, guildRaidBossId));
            }
        }

        public uint PlayerID => _record.PlayerID;
        public uint GuildRaidBossID => _record.GuildRaidBossID;
        public PlayerGuildRaidBossState State => (PlayerGuildRaidBossState)_record.State;
        public uint Damage => _record.Damage;

        /// <summary>
        /// ダメージを加算する。static メソッドの内部で利用するだけなので private メソッド。
        /// </summary>
        /// <param name="damage">与えたダメージ</param>
        void _addDamage(uint damage)
        {
            var newRecord = _cloneRecord();
            newRecord.Damage += damage;
            if (State == PlayerGuildRaidBossState.None) newRecord.State = (byte)PlayerGuildRaidBossState.Battle;
            _changeLocalRecord(newRecord);
        }
        /// <summary>
        /// 報酬受け取り可否判定。
        /// </summary>
        /// <returns></returns>
        public bool CanReceiveRewards()
        {
            return State == PlayerGuildRaidBossState.Battle;
        }
        /// <summary>
        /// 報酬を受け取ったことを記録する。
        /// </summary>
        public void ReceiveRewards()
        {
            var newRecord = _cloneRecord();
            newRecord.State = (byte)PlayerGuildRaidBossState.HasReceivedRewards;
            _changeLocalRecord(newRecord);
        }

        public Core.WebApi.Response.RaidBoss.PlayerGuildRaidBoss ToResponseData(PlayerStatusEntity playerStatus)
        {
            if (playerStatus.PlayerID != PlayerID) throw new InvalidOperationException();
            return new Core.WebApi.Response.RaidBoss.PlayerGuildRaidBoss
            {
                Player = playerStatus.ToOtherResponseData(),
                Damage = Damage,                
            };
        }
    }
}
