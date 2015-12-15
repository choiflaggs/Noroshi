using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.RaidBossGroupSchema;
using Noroshi.Server.Roles.DayOfWeekOpenable;
using Noroshi.Server.Roles.DayOfWeekOpenable.Extensions;
using Noroshi.Server.Roles.OneTimeOpenable;
using Noroshi.Server.Roles.OneTimeOpenable.Extensions;

namespace Noroshi.Server.Entity.RaidBoss
{
    /// <summary>
    /// レイドボスグループ設定を扱うクラス。
    /// </summary>
    public class RaidBossGroupEntity : AbstractDaoWrapperEntity<RaidBossGroupEntity, RaidBossGroupDao, Schema.PrimaryKey, Schema.Record>, IOneTimeOpenable, IDayOfWeekOpenable
    {
        /// <summary>
        /// ビルドする。
        /// </summary>
        /// <param name="ids">レイドボスグループ ID</param>
        /// <returns></returns>
        public static IEnumerable<RaidBossGroupEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })));
        }

        static IEnumerable<RaidBossGroupEntity> _loadAssociatedEntities(IEnumerable<RaidBossGroupEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var characterMap = CharacterEntity.ReadAndBuildMulti(entities.Select(e => e.CharacterID)).ToDictionary(rb => rb.ID);
            return entities.Select(entity =>
            {
                entity._setCharacter(characterMap[entity.CharacterID]);
                return entity;
            });
        }


        CharacterEntity _character;

        void _setCharacter(CharacterEntity character)
        {
            if (CharacterID != character.ID) throw new InvalidOperationException();
            _character = character;
        }

        /// <summary>
        /// レイドボスグループ ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey => string.IsNullOrEmpty(_record.TextKey) ? _character.TextKey : _record.TextKey;
        /// <summary>
        /// レイドボス種別。
        /// </summary>
        public RaidBossGroupType Type => (RaidBossGroupType)_record.Type;
        /// <summary>
        /// キャラクター ID。
        /// </summary>
        public uint CharacterID => _record.CharacterID;
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
        /// オープン中かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsOpen() { return this.IsOneTimeOpen() && this.IsDayOfWeekOpen(); }
    }
}
