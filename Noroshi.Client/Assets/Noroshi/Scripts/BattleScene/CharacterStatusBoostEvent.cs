using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene
{
    public class CharacterStatusBoostEvent
    {
        public enum EventType
        {
            Add,
            Remove
        }
        public EventType Type;
        public StatusBooster StatusBooster;
        public IStatusBoostFactor StatusBoosterFactor;
    }
}
