using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Quest;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.QuestSchema;

namespace Noroshi.Server.Entity.Quest
{
    public class QuestEntity : AbstractDaoWrapperEntity<QuestEntity, QuestDao, Schema.PrimaryKey, Schema.Record>, IQuestEntity
    {
        public static IEnumerable<QuestEntity> ReadAndBuildAll()
        {
            return _loadAssociatedEntities((new QuestDao()).ReadAll().Select(r => _instantiate(r)));
        }
        public static IEnumerable<QuestEntity> ReadAndBuildByTriggerID(uint triggerId)
        {
            return _loadAssociatedEntities((new QuestDao()).ReadByTriggerID(triggerId).Select(r => _instantiate(r)));
        }
        public static QuestEntity ReadAndBuild(uint questId)
        {
            var entity = ReadAndBuild(new Schema.PrimaryKey { ID = questId });
            if (entity == null) return null;
            return _loadAssociatedEntities(new QuestEntity[] { entity }).First();
        }
        static IEnumerable<QuestEntity> _loadAssociatedEntities(IEnumerable<QuestEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var rewards = QuestRewardEntity.ReadAndBuildByQuestIDs(entities.Select(e => e.ID));
            var rewardLookup = rewards.ToLookup(reward => reward.QuestID);
            return entities.Select(entity =>
            {
                entity._setRewards(rewardLookup[entity.ID]);
                return entity;
            });
        }

        IEnumerable<QuestRewardEntity> _rewards;

        void _setRewards(IEnumerable<QuestRewardEntity> rewards)
        {
            _rewards = rewards;
        }

        public uint ID => _record.ID;
        public string TextKey => "Master.Quest." + _record.TextKey;
        public uint TriggerID => _record.TriggerID;
        public uint Threshold => _record.Threshold;
        public uint? GameContentID => (new QuestTriggerEntity(TriggerID)).GameContentID;

        public bool IsOpen => true;

        public IEnumerable<PossessionParam> GetPossessionParams()
        {
            return _rewards.Select(reward => reward.GetPossessableParam());
        }

        public Core.WebApi.Response.Quest.Quest ToResponseData<T>(IPlayerQuestTriggerEntity<T> playerTrigger, IEnumerable<IPossessionObject> possessionObjects)
            where T : IQuestEntity
        {
            return new Core.WebApi.Response.Quest.Quest
            {
                ID = ID,
                GameContentID = GameContentID.HasValue ? GameContentID.Value : 0,
                Current = playerTrigger != null ? playerTrigger.CurrentNum : 0,
                Threshold = Threshold,
                CanReceiveReward = playerTrigger != null ? playerTrigger.CanReceiveReward(Threshold) : false,
                HasAlreadyReceivedReward = playerTrigger != null ? playerTrigger.HasAlreadyReceivedReward(Threshold) : false,
                TextKey = TextKey,
                PossessionObjects = possessionObjects.Select(po => po.ToResponseData()).ToArray(),
            };
        }
    }
}
