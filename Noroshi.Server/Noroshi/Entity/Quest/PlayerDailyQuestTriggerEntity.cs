using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerDailyQuestTriggerSchema;
using Noroshi.Server.Daos.Rdb.Quest;

namespace Noroshi.Server.Entity.Quest
{
    public class PlayerDailyQuestTriggerEntity : AbstractDaoWrapperEntity<PlayerDailyQuestTriggerEntity, PlayerDailyQuestTriggerDao, Schema.PrimaryKey, Schema.Record>, IPlayerQuestTriggerEntity<DailyQuestEntity>
    {
        public static IEnumerable<PlayerDailyQuestTriggerEntity> ReadAndBuildByPlayerID(uint playerId)
        {
            return (new PlayerDailyQuestTriggerDao()).ReadByPlayerID(playerId).Select(r => _instantiate(r));
        }

        public static PlayerDailyQuestTriggerEntity ReadAndBuild(uint playerId, uint triggerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId, TriggerID = triggerId }, readType);
        }

        public static PlayerDailyQuestTriggerEntity CreateOrReadAndBuild(uint playerId, uint triggerId, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerDailyQuestTriggerDao()).CreateOrRead(playerId, triggerId, 1));
        }

        public static PlayerDailyQuestTriggerEntity CountUp(uint playerId, uint triggerId, uint add = 1)
        {
            var record = (new PlayerDailyQuestTriggerDao()).Create(playerId, triggerId, add);
            PlayerDailyQuestTriggerEntity entity = null;
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

        public static IEnumerable<PlayerDailyQuestTriggerEntity> ReadAndBuildByPlayerIDAndTimedQuestCompensate(uint playerId)
        {
            // 初回の時限型のデイリークエストはユーザレコードが存在しないのでビルドして補う.

            var dailyQuestEntitys = DailyQuestEntity.ReadAndBuildAll();
            var playerDailyQuestTriggerEntrys = ReadAndBuildByPlayerID(playerId);

            List<PlayerDailyQuestTriggerEntity> makePlayerDailyQuestTriggerEntrys = new List<PlayerDailyQuestTriggerEntity>();
            foreach (var dailyQuestEntity in dailyQuestEntitys)
            {
                if (!QuestTriggerEntity.IsQuestTypeTimeZone(dailyQuestEntity.TriggerID) || dailyQuestEntity.IsOpen == false) continue;

                // 時限式が存在しなければビルドして補完　すでにレコードがあれば通常フローで対応できる為スルーする.
                if (playerDailyQuestTriggerEntrys.Count() == 0 || playerDailyQuestTriggerEntrys.Any(playerDailyQuest => playerDailyQuest.TriggerID == dailyQuestEntity.TriggerID) == false)
                {
                    makePlayerDailyQuestTriggerEntrys.Add(_instantiate((new PlayerDailyQuestTriggerDao()).GetDefaultRecord(playerId, dailyQuestEntity.TriggerID, 1)));
                }
            }
            return playerDailyQuestTriggerEntrys.Concat(makePlayerDailyQuestTriggerEntrys).OrderBy(playerDaily => playerDaily.TriggerID);
        }

        public static PlayerDailyQuestTriggerEntity CreateOrReadAndBuildAndTimedQuestCompensate(uint playerId, uint triggerId, ReadType readType = ReadType.Slave)
        {
            // 時限式クエストはCountUpしないのでレコードが存在しない可能性がある.
            if (QuestTriggerEntity.IsQuestTypeTimeZone(triggerId))
            {
                // 時間帯以外に不正に受け取ろうとしているケースを想定した確認.
                var timedQuest = DailyQuestEntity.ReadAndBuildByTriggerID(triggerId).FirstOrDefault();
                if(timedQuest != null && timedQuest.IsOpen) return _instantiate((new PlayerDailyQuestTriggerDao()).CreateOrRead(playerId, triggerId, 1));
            }
            return ReadAndBuild(playerId, triggerId, readType);
        }



        public uint TriggerID => _record.TriggerID;
        public uint CurrentNum => _getCurrentNum();
        public uint ReceiveRewardThreshold => _getReceiveRewardThreshold();

        uint _getCurrentNum()
        {
            return ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.UpdatedAt) ? 0 : _record.CurrentNum;
        }
        uint _getReceiveRewardThreshold()
        {
            return ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.UpdatedAt) ? 0 : _record.ReceiveRewardThreshold;
        }

        public DailyQuestEntity GetCurrentQuest(IEnumerable<DailyQuestEntity> sameTriggerDailyQuests)
        {
            return sameTriggerDailyQuests.Where(q => q.Threshold > ReceiveRewardThreshold).OrderBy(q => q.Threshold).FirstOrDefault();
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
