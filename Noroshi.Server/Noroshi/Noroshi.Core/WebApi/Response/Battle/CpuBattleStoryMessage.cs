using Noroshi.Core.Game.Battle;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public class CpuBattleStoryMessage
    {
        public byte No { get; set; }
        public string TextKey { get; set; }
        public uint? OwnCharacterID { get; set; }
        public StoryCharacterActingType? OwnCharacterActingType { get; set; }
        public uint? EnemyCharacterID { get; set; }
        public StoryCharacterActingType? EnemyCharacterActingType { get; set; }
        public StoryEffectType? EffectType { get; set; }
    }
}
