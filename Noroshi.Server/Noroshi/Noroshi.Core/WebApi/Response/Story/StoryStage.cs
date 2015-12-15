using Noroshi.Core.Game.Story;
using Noroshi.Core.WebApi.Response.Battle;

namespace Noroshi.Core.WebApi.Response.Story
{
    public class StoryStage
    {
        public uint ID { get; set; }
        public ushort No { get; set; }
        public uint EpisodeID { get; set; }
        public Enums.StageType Type { get; set; }
        public uint BattleID { get; set; }
        public byte IsFixedParty { get; set; }
        public uint[] FixedCharacterIDs { get; set; }
        public uint[] CpuCharacterIDs { get; set; }
        public string TextKey { get; set; }
        public ushort Stamina { get; set; }
        public CpuBattle Battle { get; set; }
    }
}
