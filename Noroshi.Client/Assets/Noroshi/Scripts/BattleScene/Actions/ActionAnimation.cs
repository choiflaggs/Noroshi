using System.Collections.Generic;

namespace Noroshi.BattleScene.Actions
{
    public class ActionAnimation
    {
        public ActionAnimation(string name, float[] triggerTimes, float finishTime)
        {
            Name = name;
            TriggerTimes = triggerTimes;
            FinishTime = finishTime;
        }
        public readonly string Name;
        public readonly float[] TriggerTimes;
        public readonly float FinishTime;
    }
}