using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Quest;

namespace Noroshi.Server.Entity.Quest
{
    public class PlayerQuestTriggerEntity : AbstractDaoWrapperEntity<PlayerQuestTriggerEntity, PlayerQuestTriggerDao, PlayerQuestTriggerDao.PrimaryKey, PlayerQuestTriggerDao.Record>, IPlayerQuestTriggerEntity<QuestEntity>
    {
        public static IEnumerable<PlayerQuestTriggerEntity> ReadAndBuildByPlayerID(uint playerId)
        {
            return (new PlayerQuestTriggerDao()).ReadByPlayerID(playerId).Select(r => _instantiate(r));
        }
        public static PlayerQuestTriggerEntity ReadAndBuild(uint playerId, uint triggerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new PlayerQuestTriggerDao.PrimaryKey { PlayerID = playerId, TriggerID = triggerId }, readType);
        }

        public static PlayerQuestTriggerEntity CountUp(uint playerId, uint triggerId, uint add = 1)
        {
            var record = (new PlayerQuestTriggerDao()).Create(playerId, triggerId, add);
            PlayerQuestTriggerEntity entity = null;
            if (record != null)
            {
                entity = _instantiate(record);
            }
            else
            {
                entity = ReadAndBuild(playerId, triggerId, ReadType.Lock);
                entity.SaveCurrent(entity.CurrentNum + add);
            }
            return entity;
        }


        public uint TriggerID => _record.TriggerID;
        public uint CurrentNum => _record.CurrentNum;
        public uint ReceiveRewardThreshold => _record.ReceiveRewardThreshold;

        public QuestEntity GetCurrentQuest(IEnumerable<QuestEntity> sameTriggerQuests)
        {
            return sameTriggerQuests.Where(q => q.Threshold > ReceiveRewardThreshold).OrderBy(q => q.Threshold).FirstOrDefault();
        }
        public bool SaveCurrent(uint current)
        {
            var newRecord = _cloneRecord();
            newRecord.CurrentNum = current;
            newRecord.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            return Save();
        }
        public bool HasAlreadyReceivedReward(uint threshold)
        {
            return ReceiveRewardThreshold >= threshold;
        }

        public bool CanReceiveReward(uint threshold)
        {
            return !HasAlreadyReceivedReward(threshold) && threshold <= CurrentNum;
        }
        public bool ReceiveReward(uint threshold)
        {
            var newRecord = _cloneRecord();
            newRecord.ReceiveRewardThreshold = threshold;
            newRecord.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            return Save();
        }
    }
}
