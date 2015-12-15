using Noroshi.Server.Contexts;
using Noroshi.Server.Services;
using Noroshi.Core.WebApi.Response.Training;

namespace Noroshi.Server.Controllers
{
    public class Training : AbstractController
    {
        public ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return TrainingService.List(playerId);
        }
    }
}
