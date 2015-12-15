using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public class CpuBattle : IBattle
    {
        public uint ID { get; set; }

        public BattleWave[] Waves { get; set; }
        public uint FieldID { get; set; }
        public uint CharacterExp { get; set; }
        public uint Gold { get; set; }
        public PossessionObject[] DroppablePossessionObjects { get; set; }

        public CpuBattleStory BeforeBattleStory { get; set; }
        public CpuBattleStory BeforeBossWaveStory { get; set; }
        public CpuBattleStory AfterBossDieStory { get; set; }
        public CpuBattleStory AfterBattleStory { get; set; }
    }
}
