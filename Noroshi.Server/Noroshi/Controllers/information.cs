using System.Linq;
using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Information : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Information.ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return InformationService.List(playerId);
        }
        [Post]
        public Core.WebApi.Response.Information.ReadResponse Read(ushort[] informationCategories)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;

            return InformationService.Read(playerId, informationCategories.Select(ic => (byte)ic).ToArray());
        }
    }
}
