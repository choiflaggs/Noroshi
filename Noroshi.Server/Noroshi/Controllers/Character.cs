using System.Linq;

namespace Noroshi.Server.Controllers
{
    public class Character : AbstractController
    {
        public Core.WebApi.Response.Character.Character[] MasterData()
        {
            return Entity.Character.CharacterEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}