using System.Collections.Generic;

namespace Noroshi.Server.Entity.Quest
{
    public interface IPlayerQuestTriggerEntity<T> where T : IQuestEntity
    {
        uint TriggerID { get; }
        uint CurrentNum { get; }
        T GetCurrentQuest(IEnumerable<T> sameTriggerQuests);
        bool HasAlreadyReceivedReward(uint threshold);
        bool CanReceiveReward(uint threshold);
        bool ReceiveReward(uint threshold);
    }
}
