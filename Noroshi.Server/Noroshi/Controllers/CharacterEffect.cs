using System.Linq;

namespace Noroshi.Server.Controllers
{
    public class CharacterEffect : AbstractController
    {
        public Core.WebApi.Response.CharacterEffect[] MasterData()
        {
            return Entity.Character.CharacterEffectEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}