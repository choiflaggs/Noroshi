using System.Linq;

namespace Noroshi.Server.Controllers
{
    public class ShadowCharacter : AbstractController
    {
        public Core.WebApi.Response.Battle.ShadowCharacter[] MasterData()
        {
            return Entity.Battle.ShadowCharacter.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}
