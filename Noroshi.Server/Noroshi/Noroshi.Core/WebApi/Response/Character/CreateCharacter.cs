using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Character
{
    public class CreateCharacter
    {
        public PossessionObject CreateCharacterPossessionObject
        { get; set; }
        public PossessionObject UseSoulPossessionObject
        { get; set; }
        public PossessionObject UseGoldPossessionObject
        { get; set; }
    }
}