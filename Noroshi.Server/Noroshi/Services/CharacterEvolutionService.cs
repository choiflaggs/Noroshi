using System.Linq;
using Noroshi.Core.WebApi.Response.Character;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Services
{
    public class CharacterEvolutionService
    {
        public static CharacterEvolutionType[] MasterData()
        {
            return CharacterEvolutionTypeEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }
    }
}