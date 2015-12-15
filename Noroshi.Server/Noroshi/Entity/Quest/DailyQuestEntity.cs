using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DailyQuestSchema;
using Noroshi.Server.Daos.Rdb.Quest;

namespace Noroshi.Server.Entity.Quest
{
    public class DailyQuestEntity : AbstractDaoWrapperEntity<DailyQuestEntity, DailyQuestDao, Schema.PrimaryKey, Schema.Record>, IQuestEntity
    {
        public static IEnumerable<DailyQuestEntity> ReadAndBuildAll()
        {
            return _loadAssociatedEntities((new DailyQuestDao()).ReadAll().Select(r => _instantiate(r)));
        }
        public static IEnumerable<DailyQuestEntity> ReadAndBuildByTriggerID(uint triggerId)
        {
            return _loadAssociatedEntities((new DailyQuestDao()).ReadByTriggerID(triggerId).Select(r => _instantiate(r)));
        }
        public static DailyQuestEntity ReadAndBuild(uint dailyQuestId)
        {
            var entity = ReadAndBuild(new Schema.PrimaryKey { ID = dailyQuestId });
            if (entity == null) return null;
            return _loadAssociatedEntities(new DailyQuestEntity[] { entity }).First();
        }
        static IEnumerable<DailyQuestEntity> _loadAssociatedEntities(IEnumerable<DailyQuestEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var rewards = DailyQuestRewardEntity.ReadAndBuildByDailyQuestIDs(entities.Select(e => e.ID));
            var rewardLookup = rewards.ToLookup(reward => reward.DailyQuestID);
            return entities.Select(entity =>
            {
                entity._setRewards(rewardLookup[entity.ID]);
                return entity;
            });
        }

        IEnumerable<DailyQuestRewardEntity> _rewards;

        void _setRewards(IEnumerable<DailyQuestRewardEntity> rewards)
        {
            _rewards = rewards;
        }

        public uint ID => _record.ID;
        public string TextKey => "Master.DailyQuest." + _record.TextKey;
        public uint TriggerID => _record.TriggerID;
        public uint Threshold => _record.Threshold;
        public uint? GameContentID => (new QuestTriggerEntity(TriggerID)).GameContentID;


        public uint OpenHour  => _record.OpenHour;
        public uint CloseHour => _record.CloseHour;

        public bool IsOpen
        {
            get
            {
                // 特定時間帯のみ公開状態になるクエストか否か.
                if ( QuestTriggerEntity.IsQuestTypeTimeZone( _record.TriggerID ) )
                {
                    // 比較値が両方初期値の場合、値があっても値が全く同じで一瞬も公開しないクエストは弾く.
                    if ( ( _record.OpenHour == 0 && _record.CloseHour == 0 ) || ( _record.OpenHour == _record.CloseHour ) ) return false;

                    // 現在時刻が 開始時間より大きく終了時間より小さい場合は許可.
                    return ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC + TimeSpan.FromHours(_record.OpenHour).TotalSeconds < ContextContainer.GetContext().TimeHandler.UnixTime
                        && ContextContainer.GetContext().TimeHandler.UnixTime < ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC + TimeSpan.FromHours(_record.CloseHour).TotalSeconds;
                }

                return true;
            }
        }

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
