using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class GearPiece : AbstractController
    {
        [Get]
        public Core.WebApi.Response.GearPiece[] MasterData() => GearPieceService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerGearPiece[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GearPieceService.GetAll(playerId);
        }
    }
}