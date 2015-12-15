using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class CharacterEvolutionLevelUpResponse
    {
        public PossessionObject UsedSoul
        { get; set; }
        public PossessionObject UsedGold
        { get; set; }
        public PlayerCharacter PlayerCharacter
        { get; set; }
    }
}