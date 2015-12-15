using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class GearEnchantAddExpResponse
    {
        public PossessionObject[] UseItemObjects { get; set; }
        public PossessionObject UseGoldObjects { get; set; }
        public PlayerCharacterGear Gear { get; set; }
    }
}