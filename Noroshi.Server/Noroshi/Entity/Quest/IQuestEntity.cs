using System.Collections.Generic;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Entity.Quest
{
    public interface IQuestEntity
    {
        uint ID { get; }
        uint TriggerID { get; }
        uint Threshold { get; }
        uint? GameContentID { get; }

        bool IsOpen { get; }

        IEnumerable<PossessionParam> GetPossessionParams();

        Core.WebApi.Response.Quest.Quest ToResponseData<T>(IPlayerQuestTriggerEntity<T> playerTrigger, IEnumerable<IPossessionObject> possessionObjects)
            where T : IQuestEntity;
    }
}
