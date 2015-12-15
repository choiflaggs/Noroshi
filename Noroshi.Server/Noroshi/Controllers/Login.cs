using LightNode.Server;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Services.Player;

namespace Noroshi.Server.Controllers
{
    public class Login : AbstractController
    {
        [Post]
        public SessionData Get(string udid) => PlayerService.Login(udid);
    }
}