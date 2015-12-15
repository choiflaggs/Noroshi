using LightNode.Server;
using Noroshi.Core.WebApi.Response.Gacha;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Gacha : AbstractController
    {
        [Get]
        public EntryPointListResponse EntryPointList()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GachaService.EntryPointList(playerId);
        }
        [Post]
        public LotResponse Lot(uint gachaEntryPointId, bool free)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GachaService.Lot(playerId, gachaEntryPointId, free);
        }
        [Post]
        public LotResponse LotTutorialGacha()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return GachaService.LotTutorialGacha(playerId);
        }
    }
}
