using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class RaidTicket : AbstractController
    {
        [Get]
        public Core.WebApi.Response.RaidTicket[] MasterData() => RaidTicketService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerRaidTicket[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return RaidTicketService.GetAll(playerId);
        }
    }
}