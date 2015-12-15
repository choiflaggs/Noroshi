using LightNode.Server;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;

namespace Noroshi.Server.Controllers
{
    public class Shop : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Shop.ListResponse List()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ShopService.List(playerId);
        }
        [Post]
        public Core.WebApi.Response.Shop.BuyResponse Buy(byte shopDisplayId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ShopService.Buy(playerId, shopDisplayId);
        }
        [Post]
        public Core.WebApi.Response.Shop.UpdateMerchandisesResponse UpdateMerchandises(uint shopId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ShopService.UpdateMerchandises(playerId, shopId);
        }
        [Post]
        public Core.WebApi.Response.Shop.SellAutomaticallyResponse SellAutomatically()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return ShopService.SellAutomatically(playerId);
        }
    }
}
