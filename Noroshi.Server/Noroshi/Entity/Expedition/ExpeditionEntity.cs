using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Expedition;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExpeditionSchema;

namespace Noroshi.Server.Entity.Expedition
{
    /// <summary>
    /// 冒険設定を扱うクラス。
    /// </summary>
    public class ExpeditionEntity : AbstractDaoWrapperEntity<ExpeditionEntity, ExpeditionDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。
        /// </summary>
        /// <param name="id">冒険 ID</param>
        /// <returns></returns>
        public static ExpeditionEntity ReadAndBuild(uint id)
        {
            return ReadAndBuildMulti(new[] { id }).FirstOrDefault();
        }
        /// <summary>
        /// 複数ビルド。
        /// </summary>
        /// <param name="ids">冒険 ID</param>
        /// <returns></returns>
        public static IEnumerable<ExpeditionEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })));
        }
        /// <summary>
        /// 現在のものをビルド。
        /// </summary>
        /// <param name="clearLevel">クリア済みレベル</param>
        /// <returns></returns>
        public static ExpeditionEntity ReadAndBuildCurrentByClearLevel(byte clearLevel)
        {
            return ReadAndBuildAll().Where(expedition => expedition.Level <= clearLevel + 1)
                .OrderByDescending(expedition => expedition.Level)
                .FirstOrDefault();
        }
        /// <summary>
        /// 全レコード分ビルド。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExpeditionEntity> ReadAndBuildAll()
        {
            return _loadAssociatedEntities(_instantiate((new ExpeditionDao()).ReadAll()));
        }
        static IEnumerable<ExpeditionEntity> _loadAssociatedEntities(IEnumerable<ExpeditionEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var expeditionIdToStagesMap = ExpeditionStageEntity.ReadAndBuildByExpeditionIDs(entities.Select(entity => entity.ID)).ToLookup(stage => stage.ExpeditionID);
            return entities.Select(entity =>
            {
                entity._setStage(expeditionIdToStagesMap[entity.ID]);
                return entity;
            });
        }


        Dictionary<byte, ExpeditionStageEntity> _stepToStage;

        void _setStage(IEnumerable<ExpeditionStageEntity> stages)
        {
            if (stages.Any(stage => stage.ExpeditionID != ID)) throw new InvalidOperationException();
            _stepToStage = stages.ToDictionary(stage => stage.Step);
            // 一応、マスター虫食いミスチェックをしておく。
            for (byte step = 1; step < GetMaxStep(); step++)
            {
                if (!_stepToStage.ContainsKey(step)) throw new SystemException();
            }
        }


        /// <summary>
        /// 冒険 ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level => _record.Level;
        /// <summary>
        /// 自動回復有無。
        /// </summary>
        public bool AutomaticRecovery => _record.AutomaticRecovery > 0;

        /// <summary>
        /// ステージを取得する。
        /// </summary>
        /// <param name="step">対象ステップ</param>
        /// <returns></returns>
        public ExpeditionStageEntity GetStage(byte step)
        {
            return _stepToStage.ContainsKey(step) ? _stepToStage[step] : null;
        }
        /// <summary>
        /// 全ステージを取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExpeditionStageEntity> GetAllStages()
        {
            return _stepToStage.Values;
        }
        /// <summary>
        /// 最大ステップを取得する。
        /// </summary>
        /// <returns></returns>
        public byte GetMaxStep()
        {
            return _stepToStage.Max(kv => kv.Value.Step);
        }

        /// <summary>
        /// レスポンスデータへ変換する。
        /// </summary>
        /// <returns></returns>
        public Core.WebApi.Response.Expedition.Expedition ToResponseData()
        {
            return new Core.WebApi.Response.Expedition.Expedition
            {
                ID = ID,
                Level = Level,
                AutomaticRecovery = AutomaticRecovery,
            };
        }
    }
}
