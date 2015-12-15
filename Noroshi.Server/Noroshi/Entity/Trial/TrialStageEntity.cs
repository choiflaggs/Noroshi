using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Trial;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrialStageSchema;

namespace Noroshi.Server.Entity.Trial
{
    /// <summary>
    /// 試練ステージ設定を扱うクラス。
    /// </summary>
    public class TrialStageEntity : AbstractDaoWrapperEntity<TrialStageEntity, TrialStageDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="id">試練ステージ ID</param>
        /// <returns></returns>
        public static TrialStageEntity ReadAndBuild(uint id)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = id }));
        }
        /// <summary>
        /// 試練 ID 指定ビルド。
        /// </summary>
        /// <param name="trialIds">試練 ID</param>
        /// <returns></returns>
        public static IEnumerable<TrialStageEntity> ReadAndBuildByTrialIDs(IEnumerable<uint> trialIds)
        {
            return _loadAssociatedEntities(_instantiate((new TrialStageDao()).ReadByTrialIDs(trialIds)));
        }
        static TrialStageEntity _loadAssociatedEntities(TrialStageEntity entity)
        {
            if (entity == null) return null;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<TrialStageEntity> _loadAssociatedEntities(IEnumerable<TrialStageEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var cpuBattleMap = CpuBattleEntity.ReadAndBuildMulti(entities.Select(entity => entity.CpuBattleID)).ToDictionary(cb => cb.ID);
            return entities.Select(entity =>
            {
                entity._setCpuBattle(cpuBattleMap[entity.CpuBattleID]);
                return entity;
            });
        }


        CpuBattleEntity _cpuBattle;

        void _setCpuBattle(CpuBattleEntity cpuBattle)
        {
            if (cpuBattle.ID != CpuBattleID) throw new InvalidOperationException();
            _cpuBattle = cpuBattle;
        }


        /// <summary>
        /// 試練ステージ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 試練 ID。
        /// </summary>
        public uint TrialID => _record.TrialID;
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level => _record.Level;
        /// <summary>
        /// CPU バトル ID。
        /// </summary>
        public uint CpuBattleID => _record.CpuBattleID;

        /// <summary>
        /// オープンフラグ。
        /// </summary>
        /// <param name="trial">親試練</param>
        /// <param name="playerTrial">親試練状態</param>
        /// <returns></returns>
        public bool IsOpen(TrialEntity trial, PlayerTrialEntity playerTrial)
        {
            if (trial.ID != TrialID || playerTrial.TrialID != TrialID) throw new InvalidOperationException();
            return trial.IsOpen() && Level <= playerTrial.ClearLevel + 1;
        }

        /// <summary>
        /// ドロップし得る報酬。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetDroppableRewards()
        {
            return _cpuBattle.GetDroppableRewards();
        }

        public Core.WebApi.Response.Trial.TrialStage ToResponseData(TrialEntity trial, PlayerTrialEntity playerTrial, PlayerTrialStageEntity playerTrialStage, PossessionManager possessionManager)
        {
            return new Core.WebApi.Response.Trial.TrialStage
            {
                ID = ID,
                Level = Level,
                DroppableRewards = possessionManager.GetPossessionObjects(GetDroppableRewards()).Select(pp => pp.ToResponseData()).ToArray(),
                IsOpen = IsOpen(trial, playerTrial),
                Rank = playerTrialStage.Rank,
            };
        }
    }
}
