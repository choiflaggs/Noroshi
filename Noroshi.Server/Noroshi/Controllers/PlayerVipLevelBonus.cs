using LightNode.Server;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerVipLevelBonus : AbstractController
    {
        [Get]
        public Core.WebApi.Response.PlayerVipLevelBonus[] MasterData() => PlayerVipLevelService.MasterData();
    }
}