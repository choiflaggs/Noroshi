using LightNode.Server;
using Noroshi.Core.WebApi.Response.PresentBox;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class PresentBox : AbstractController
    {
        [Get]
        public ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PresentBoxService.List(playerId);
        }
        [Post]
        public ReceiveResponse Receive(uint[] presentBoxIds)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PresentBoxService.Receive(playerId, presentBoxIds);
        }
    }
}
