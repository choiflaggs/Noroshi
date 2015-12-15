using System.Linq;
using LightNode.Server;

namespace Noroshi.Server.Controllers
{
    public class GearRecipe : AbstractController
    {
        [Get]
        public Core.WebApi.Response.GearRecipe[] MasterData() => Entity.Item.GearRecipeEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
    }
}