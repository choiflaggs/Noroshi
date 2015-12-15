using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Character;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Trial;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.TrialSchema;
using Noroshi.Server.Roles.DayOfWeekOpenable;
using Noroshi.Server.Roles.DayOfWeekOpenable.Extensions;
using Noroshi.Server.Roles.OneTimeOpenable;
using Noroshi.Server.Roles.OneTimeOpenable.Extensions;

namespace Noroshi.Server.Entity.Trial
{
    /// <summary>
    /// 試練設定を扱うクラス。
    /// </summary>
    public class TrialEntity : AbstractDaoWrapperEntity<TrialEntity, TrialDao, Schema.PrimaryKey, Schema.Record>, IOneTimeOpenable, IDayOfWeekOpenable
    {
        public static TrialEntity ReadAndBuild(uint id)
        {
            return _loadAssociatedEntities(ReadAndBuild(new Schema.PrimaryKey { ID = id }));
        }
        /// <summary>
        /// 閲覧可能なものを全てビルド。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TrialEntity> ReadAndBuildVisibleAll()
        {
            return _loadAssociatedEntities(_instantiate((new TrialDao()).ReadAll()).Where(entity => entity.IsVisible()));
        }
        static TrialEntity _loadAssociatedEntities(TrialEntity entity)
        {
            if (entity == null) return null;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<TrialEntity> _loadAssociatedEntities(IEnumerable<TrialEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var trialIdToStagesMap = TrialStageEntity.ReadAndBuildByTrialIDs(entities.Select(entity => entity.ID)).ToLookup(stage => stage.TrialID);
            return entities.Select(entity =>
            {
                entity._setStages(trialIdToStagesMap[entity.ID]);
                return entity;
            });
        }


        IEnumerable<TrialStageEntity> _stages;

        void _setStages(IEnumerable<TrialStageEntity> stages)
        {
            if (stages.Any(stage => stage.TrialID != ID)) throw new InvalidOperationException();
            _stages = stages.OrderBy(stage => stage.Level);
        }


        /// <summary>
        /// 試練 ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 多言語対応のためのテキストキー。
        /// </summary>
        public string TextKey => "Master.Trial." + _record.TextKey;
        /// <summary>
        /// タグ制約。
        /// </summary>
        public CharacterTagSet CharacterTagSet => !string.IsNullOrEmpty(_record.TagFlags) ? new CharacterTagSet(_record.TagFlags) : null;
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
        public IEnumerable<TrialStageEntity> GetStages()
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

        public Core.WebApi.Response.Trial.Trial ToResponseData(PlayerTrialEntity playerTrial, IEnumerable<PlayerTrialStageEntity> playerTrialStages, PossessionManager possessionManager)
        {
            var playerTrialStageMap = playerTrialStages.ToDictionary(pts => pts.TrialStageID);
            return new Core.WebApi.Response.Trial.Trial
            {
                ID = ID,
                TextKey = TextKey,
                TagFlags = _record.TagFlags,
                OpenedAt = OpenedAt,
                ClosedAt = ClosedAt,
                OpenDayOfWeeks = this.GetOpenDayOfWeeks().ToArray(),
                IsOpen = IsOpen(),
                BattleNum = playerTrial.BattleNum,
                ReopenedAt = playerTrial.ReopenedAt,
                Stages = _stages.Select(stage => stage.ToResponseData(this, playerTrial, playerTrialStageMap[stage.ID], possessionManager)).ToArray(),
            };
        }
    }
}
