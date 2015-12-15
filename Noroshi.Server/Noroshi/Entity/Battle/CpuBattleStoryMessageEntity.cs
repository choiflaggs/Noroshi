using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleStoryMessageSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class CpuBattleStoryMessageEntity : AbstractDaoWrapperEntity<CpuBattleStoryMessageEntity, CpuBattleStoryMessageDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CpuBattleStoryMessageEntity> ReadAndBuildByStoryIDs(IEnumerable<uint> storyIds)
        {
            var dao = new CpuBattleStoryMessageDao();
            return dao.ReadByStoryIDs(storyIds).Select(r => _instantiate(r));
        }

        public uint StoryID => _record.StoryID;
        public byte No => _record.No;
        public string TextKey => string.IsNullOrEmpty(_record.TextKey) ? "" : "Master.CpuBattleStoryMessage." + _record.TextKey;
        public uint? OwnCharacterID => _record.OwnCharacterID > 0 ? (uint?)_record.OwnCharacterID : null;
        public StoryCharacterActingType? OwnCharacterActingType => _record.OwnCharacterActingType > 0 ? (StoryCharacterActingType?)_record.OwnCharacterActingType : null;
        public uint? EnemyCharacterID => _record.EnemyCharacterID > 0 ? (uint?)_record.EnemyCharacterID : null;
        public StoryCharacterActingType? EnemyCharacterActingType => _record.EnemyCharacterActingType > 0 ? (StoryCharacterActingType?)_record.EnemyCharacterActingType : null;
        public StoryEffectType? EffectType => _record.EffectType > 0 ? (StoryEffectType?)_record.EffectType : null;

        public Core.WebApi.Response.Battle.CpuBattleStoryMessage ToResponseData()
        {
            return new Core.WebApi.Response.Battle.CpuBattleStoryMessage()
            {
                No = No,
                TextKey = TextKey,
                OwnCharacterID = OwnCharacterID,
                OwnCharacterActingType = OwnCharacterActingType,
                EnemyCharacterID = EnemyCharacterID,
                EnemyCharacterActingType = EnemyCharacterActingType,
                EffectType = EffectType,
            };
        }
    }
}
