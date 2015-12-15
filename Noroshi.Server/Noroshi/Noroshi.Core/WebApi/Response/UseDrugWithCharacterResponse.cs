using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class UseDrugWithCharacterResponse
    {
        public PossessionObject CreateCharacterPossessionObject
        { get; set; }
        public PossessionObject UseDrugPossessionObject
        { get; set; }
        public PossessionObject UseGoldPossessionObject
        { get; set; }
    }
}