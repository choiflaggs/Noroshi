using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Item;

namespace Noroshi.Server.Controllers
{
    public class ExchangeCashGift : AbstractController
    {
        [Get]
        public Core.WebApi.Response.ExchangeCashGift[] MasterData() => ExchangeCashGiftService.MasterData();

        [Get]
        public Core.WebApi.Response.PlayerExchangeCashGift[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ExchangeCashGiftService.GetAll(playerId);
        }
    }
}