using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleStorySchema;

namespace Noroshi.Server.Entity.Battle
{
    public class CpuBattleStoryEntity : AbstractDaoWrapperEntity<CpuBattleStoryEntity, CpuBattleStoryDao, Schema.PrimaryKey, Schema.Record>
    {
        public enum StoryTrigger
        {
            BeforeBattle = 1,
            BeforeBossWave = 2,
            AfterBossDie = 3,
            AfterBattle = 4,
        }
        public static IEnumerable<CpuBattleStoryEntity> ReadAndBuildByBattleIDs(IEnumerable<uint> battleIds)
        {
            var entities = (new CpuBattleStoryDao()).ReadByBattleIDs(battleIds).Select(r => _instantiate(r));
            if (entities.Count() == 0)
            {
                return entities;
            }
            var storyIdTomessage = CpuBattleStoryMessageEntity.ReadAndBuildByStoryIDs(entities.Select(e => e.ID))
                .ToLookup(message => message.StoryID);
            return entities.Select(e =>
            {
                e.SetMessages(storyIdTomessage[e.ID]);
                return e;
            });
        }

        IEnumerable<CpuBattleStoryMessageEntity> _messages;

        public uint ID => _record.ID;
        public StoryTrigger Trigger => (StoryTrigger)_record.TriggerID;
        public uint BattleID => _record.BattleID;

        public void SetMessages(IEnumerable<CpuBattleStoryMessageEntity> messages)
        {
            _messages = messages;
        }

        public Core.WebApi.Response.Battle.CpuBattleStory ToResponseData()
        {
            return new Core.WebApi.Response.Battle.CpuBattleStory()
            {
                Messages = _messages.Select(message => message.ToResponseData()).ToArray(),
                DramaType = _record.DramaType,
            };
        }
    }
}
