using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class PlayerCharacterChangeGearResponse
    {
        public string SessionID { get; set; }
        public PossessionObject PlayerGearObject { get; set; }
        public PlayerCharacter PlayerCharacter { get; set; }
    }
}