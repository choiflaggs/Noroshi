using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public class PlayerBattle : IBattle
    {
        public BattleWave[] Waves { get; set; }
        public uint FieldID { get; set; }
        public uint CharacterExp { get; set; }
        public uint Gold { get; set; }
        public PossessionObject[] DroppablePossessionObjects { get; set; }
    }
}
