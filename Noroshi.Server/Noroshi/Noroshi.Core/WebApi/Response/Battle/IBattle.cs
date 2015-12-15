using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public interface IBattle
    {
        BattleWave[] Waves { get; }
        uint FieldID { get; }
        uint CharacterExp { get; }
        uint Gold { get; }
        PossessionObject[] DroppablePossessionObjects { get; }
    }
}
