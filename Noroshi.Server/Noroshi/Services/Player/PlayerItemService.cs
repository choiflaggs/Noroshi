using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;

namespace Noroshi.Server.Services.Player
{
    public class PlayerItemService
    {
        public static PlayerItem[] GetAll()
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerItemEntity.ReadAndBuildMultiByPlayerID(playerId)
                .Select(e => e.ToResponseData()).ToArray();
        }

        public static PlayerItem Get(uint itemId)
        {
            var playerId = ContextContainer.GetWebContext().Player.ID;
            return PlayerItemEntity.ReadAndBuildByPlayerIDAndItemID(playerId, itemId).ToResponseData();
        }
    }
}