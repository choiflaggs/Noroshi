using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Services.Item
{
    public class ExchangeCashGiftService
    {
        public static PlayerExchangeCashGift[] GetAll(uint playerId)
        {
            return PlayerExchangeCashGiftEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static ExchangeCashGift[] MasterData()
        {
            return ExchangeCashGiftEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }
    }
}