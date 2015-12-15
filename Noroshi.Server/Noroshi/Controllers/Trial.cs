using Noroshi.Server.Contexts;
using Noroshi.Server.Services;
using Noroshi.Core.WebApi.Response.Trial;

namespace Noroshi.Server.Controllers
{
    public class Trial : AbstractController
    {
        public ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return TrialService.List(playerId);
        }
    }
}
