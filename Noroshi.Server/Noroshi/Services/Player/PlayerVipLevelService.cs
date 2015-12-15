using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Shop;

namespace Noroshi.Server.Services.Player
{
    public class PlayerVipLevelService
    {
        public static PlayerVipLevelBonus[] MasterData()
        {
            var shops = ShopEntity.ReadAndBuildTemporaryShops().ToArray();
            return PlayerVipLevelBonusEntity.ReadAndBuildAll().Select(vip => vip.ToResponseData(shops)).ToArray(); 
        }
    }
}