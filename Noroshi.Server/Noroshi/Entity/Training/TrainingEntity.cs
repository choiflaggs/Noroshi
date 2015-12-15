using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Training;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Training;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrainingSchema;
using Noroshi.Server.Roles.DayOfWeekOpenable;
using Noroshi.Server.Roles.DayOfWeekOpenable.Extensions;
using Noroshi.Server.Roles.OneTimeOpenable;
using Noroshi.Server.Roles.OneTimeOpenable.Extensions;

namespace Noroshi.Server.Entity.Training
{
    /// <summary>
    /// 修行設定を扱うクラス。
    /// </summary>
    public class TrainingEntity : AbstractDaoWrapperEntity<TrainingEntity, TrainingDao, Schema.PrimaryKey, Schema.Record>, IOneTimeOpenable, IDayOfWeekOpenable
    {
        public static TrainingEntity ReadAndBuild(uint id)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = id }));
        }
        /// <summary>
        /// 閲覧可能なものを全てビルド。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TrainingEntity> ReadAndBuildVisibleAll()
        {
            return _loadAssociatedEntities(_instantiate((new TrainingDao()).ReadAll()).Where(entity => entity.IsVisible()));
        }
        static TrainingEntity _loadAssociatedEntities(TrainingEntity entity)
        {
            if (entity == null) return null;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<TrainingEntity> _loadAssociatedEntities(IEnumerable<TrainingEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var trainingIdToStagesMap = TrainingStageEntity.ReadAndBuildByTrainingIDs(entities.Select(entity => entity.ID)).ToLookup(stage => stage.TrainingID);
            return entities.Select(entity =>
            {
                entity._setStages(trainingIdToStagesMap[entity.ID]);
                return entity;
            });
        }


        IEnumerable<TrainingStageEntity> _stages;

        void _setStages(IEnumerable<TrainingStageEntity> stages)
        {
            if (stages.Any(stage => stage.TrainingID != ID)) throw new InvalidOperationException();
            _stages = stages.OrderBy(stage => stage.Level);
        }


        /// <summary>
        /// 修行 ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 多言語対応のためのテキストキー。
        /// </summary>
        public string TextKey => "Master.Training." + _record.TextKey;
        /// <summary>
        /// 修行種別。
        /// </summary>
        public TrainingType Type => (TrainingType)_record.Type;
        /// <summary>
        /// 開始日時。
        /// </summary>
        public uint? OpenedAt => _record.OpenedAt > 0 ? (uint?)_record.OpenedAt : null;
        /// <summary>
        /// 終了日時。
        /// </summary>
        public uint? ClosedAt => _record.ClosedAt > 0 ? (uint?)_record.ClosedAt : null;

        public bool IsOpenOnSunday => _record.Sunday > 0;
        public bool IsOpenOnMonday => _record.Monday > 0;
        public bool IsOpenOnTuesday => _record.Tuesday > 0;
        public bool IsOpenOnWednesday => _record.Wednesday > 0;
        public bool IsOpenOnThursday => _record.Thursday > 0;
        public bool IsOpenOnFriday => _record.Friday > 0;
        public bool IsOpenOnSaturday => _record.Saturday > 0;

        /// <summary>
        /// 紐付いているステージを取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TrainingStageEntity> GetStages()
        {
            return _stages;
        }

        /// <summary>
        /// 閲覧可能かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsVisible() { return this.IsOneTimeOpen(); }
        /// <summary>
        /// オープン中かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsOpen() { return this.IsOneTimeOpen() && this.IsDayOfWeekOpen(); }

        public Core.WebApi.Response.Training.Training ToResponseData(ushort playerLevel, PlayerTrainingEntity playerTraining, IEnumerable<PlayerTrainingStageEntity> playerTrainingStages)
        {
            var playerTrainingStageMap = playerTrainingStages.ToDictionary(pts => pts.TrainingStageID);
            return new Core.WebApi.Response.Training.Training
            {
                ID = ID,
                TextKey = TextKey,
                Type = Type,
                OpenedAt = OpenedAt,
                ClosedAt = ClosedAt,
                OpenDayOfWeeks = this.GetOpenDayOfWeeks().ToArray(),
                IsOpen = IsOpen(),
                BattleNum = playerTraining.BattleNum,
                ReopenedAt = playerTraining.ReopenedAt,
                Stages = _stages.Select(stage => stage.ToResponseData(this, playerLevel, playerTrainingStageMap[stage.ID])).ToArray(),
            };
        }
    }
}
