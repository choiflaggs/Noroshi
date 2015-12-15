using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossSchema;

namespace Noroshi.Server.Entity.RaidBoss
{
    /// <summary>
    /// レイドボス設定を扱うクラス。
    /// </summary>
    public class RaidBossEntity : AbstractDaoWrapperEntity<RaidBossEntity, RaidBossDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// ビルドする。
        /// </summary>
        /// <param name="ids">レイドボス ID</param>
        /// <returns></returns>
        public static IEnumerable<RaidBossEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })));
        }

        /// <summary>
        /// 通常ボスを抽選する。
        /// </summary>
        /// <param name="guildClusterId">ギルドクラスター</param>
        /// <param name="activeRaidBossNum">出現中レイドボス数</param>
        /// <returns></returns>
        public static RaidBossEntity LotNormal(uint guildClusterId, byte activeRaidBossNum)
        {
            var candidates = _loadAssociatedEntities(_instantiate((new RaidBossDao()).ReadByGuildClusterID(guildClusterId))).Where(entity => entity.IsOpen() && entity.Type == RaidBossGroupType.Normal);
            // 現出現レイドボス数による確率補正値。
            var coefficient = (float)Math.Pow(Constant.ACTIVE_RAID_BOSS_NUM_APPEARANCE_PROBABILITY_COEFFICIENT, activeRaidBossNum);
            return ContextContainer.GetContext().RandomGenerator.LotWithProbability(candidates, entity => entity.EncounterProbability * coefficient);
        }
        /// <summary>
        /// 巨大ボスを抽選する。
        /// </summary>
        /// <param name="defeatRaidBossLevel">撃破レイドボスレベル</param>
        /// <param name="forceHit">確実に抽選できるフラグ</param>
        /// <returns></returns>
        public static RaidBossEntity LotSpecial(byte defeatRaidBossLevel, bool forceHit)
        {
            var candidates = _loadAssociatedEntities(_instantiate((new RaidBossDao()).ReadByLevel(defeatRaidBossLevel))).Where(entity => entity.IsOpen() && entity.Type == RaidBossGroupType.Special);
            return forceHit ? ContextContainer.GetContext().RandomGenerator.Lot(candidates, entity => entity.EncounterProbability) : ContextContainer.GetContext().RandomGenerator.LotWithProbability(candidates, entity => entity.EncounterProbability);
        }
        static IEnumerable<RaidBossEntity> _loadAssociatedEntities(IEnumerable<RaidBossEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var groupMap = RaidBossGroupEntity.ReadAndBuildMulti(entities.Select(entity => entity.GroupID)).ToDictionary(rbg => rbg.ID);
            var cpuBattleMap = CpuBattleEntity.ReadAndBuildMulti(entities.Select(entity => entity.CpuBattleID)).ToDictionary(cb => cb.ID);
            var rewards = RaidBossRewardEntity.ReadAndBuildByRaidBossIDs(entities.Select(e => e.ID));
            var rewardLookup = rewards.ToLookup(reward => reward.RaidBossID);
            return entities.Select(entity =>
            {
                entity._setAssociatedEntities(groupMap[entity.GroupID], cpuBattleMap[entity.CpuBattleID], rewardLookup[entity.ID]);
                return entity;
            });
        }


        RaidBossGroupEntity _group;
        CpuBattleEntity _cpuBattle;
        IEnumerable<RaidBossRewardEntity> _rewards;

        void _setAssociatedEntities(RaidBossGroupEntity group, CpuBattleEntity cpuBattle, IEnumerable<RaidBossRewardEntity> rewards)
        {
            if (group.ID != GroupID) throw new InvalidOperationException();
            if (cpuBattle.ID != CpuBattleID) throw new InvalidOperationException();
            if (rewards.Any(reward => reward.RaidBossID != ID)) throw new InvalidOperationException();
            _group = group;
            _cpuBattle = cpuBattle;
            _rewards = rewards;
        }

        /// <summary>
        /// レイドボス ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// レイドボスグループ ID。
        /// </summary>
        public uint GroupID => _record.GroupID;
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level => _record.Level;
        /// <summary>
        /// CPU バトル ID。
        /// </summary>
        public uint CpuBattleID => _record.CpuBattleID;
        /// <summary>
        /// 出現確率。
        /// </summary>
        public float EncounterProbability => _record.EncounterProbability;
        /// <summary>
        /// 生存期間（出現後、この期間を過ぎると逃走する）。
        /// </summary>
        public TimeSpan Lifetime => TimeSpan.FromMinutes(_record.LifetimeMinute);

        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey => _group.TextKey;
        /// <summary>
        /// レイドボス種別。
        /// </summary>
        public RaidBossGroupType Type => _group.Type;
        /// <summary>
        /// クローズ日時。
        /// </summary>
        public uint? ClosedAt => _group.ClosedAt;
        /// <summary>
        /// オープン中かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsOpen() => _group.IsOpen();

        /// <summary>
        /// CPU キャラクターを取得する。
        /// </summary>
        /// <param name="minWaveNo">最低ウェーブ番号</param>
        /// <returns></returns>
        public IEnumerable<CpuCharacterEntity> GetCpuCharacters(byte minWaveNo = 1)
        {
            return _cpuBattle.GetCpuCharacters(minWaveNo);
        }
        /// <summary>
        /// 全レイドボス報酬を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RaidBossRewardEntity> GetAllRewards()
        {
            return new List<RaidBossRewardEntity>(_rewards);
        }
        /// <summary>
        /// 発見者レイドボス報酬を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RaidBossRewardEntity> GetDiscoveryRewards()
        {
            return _getRewardss(RaidBossRewardCategory.Discovery);
        }
        /// <summary>
        /// 参加レイドボス報酬を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RaidBossRewardEntity> GetEntryRewards()
        {
            return _getRewardss(RaidBossRewardCategory.Entry);
        }
        IEnumerable<RaidBossRewardEntity> _getRewardss(RaidBossRewardCategory rewardCategory)
        {
            return _rewards.Where(reward => reward.Category == rewardCategory);
        }
    }
}
