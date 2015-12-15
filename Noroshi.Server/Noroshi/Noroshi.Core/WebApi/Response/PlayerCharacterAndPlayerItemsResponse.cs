using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class PlayerCharacterAndPlayerItemsResponse
    {
        public PlayerCharacter PlayerCharacter { get; set; }
        public PossessionObject[] GettingGearEnchantMaterials { get; set; }
    }
}