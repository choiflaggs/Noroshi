using LightNode.Server;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class PlayerArena : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Arena.PlayerArena Get() => PlayerArenaService.GetPlayerArena();

        [Get]
        public Core.WebApi.Response.Players.PlayerArenaOtherResponse[] GetBattleCandidates() => PlayerArenaService.GetBattleCandidates();

        [Post]
        public Core.WebApi.Response.Arena.PlayerArena ChangeDeck(uint[] characterIds) => PlayerArenaService.ChangeDeck(characterIds);

        [Get]
        public Core.WebApi.Response.Arena.ArenaServiceResponse ResetCooltime() => PlayerArenaService.ResetCoolTime();


        [Get]
        public Core.WebApi.Response.Arena.ArenaServiceResponse ResetPlayNum() => PlayerArenaService.ResetPlayNum();


    }
}