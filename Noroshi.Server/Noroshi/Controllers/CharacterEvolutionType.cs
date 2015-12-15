using LightNode.Server;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class CharacterEvolutionType : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Character.CharacterEvolutionType[] MasterData() => CharacterEvolutionService.MasterData();

    }
}