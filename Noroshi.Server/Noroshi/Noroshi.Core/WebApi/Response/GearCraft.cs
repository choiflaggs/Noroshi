using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class GearCraft
    {
        public PossessionObject CreateGearPossessionObject
        { get; set; }

        public PossessionObject[] UseMaterialPossessionObjects
        { get; set; }
    }
}