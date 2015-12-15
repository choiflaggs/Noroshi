using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class CharacterExpDopingResponse
    {
        public PossessionObject UsedDrug { get; set; }
        public PlayerCharacter PlayerCharacter { get; set; }
    }
}