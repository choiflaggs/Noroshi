using System.Linq;

namespace Noroshi.Server.Controllers
{
    public class Attribute : AbstractController
    {
        public Core.WebApi.Response.Character.Attribute[] MasterData()
        {
            return Entity.Character.AttributeEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }
    }
}