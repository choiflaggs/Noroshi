using System.Linq;

namespace Noroshi.Server.Controllers
{
    public class Action : AbstractController
    {
        public Core.WebApi.Response.Character.Action[] MasterData()
        {
            return Entity.Character.ActionEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}