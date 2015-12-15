using LightNode.Server;
using Noroshi.Server.Services.Debug;

namespace Noroshi.Server.Controllers.Debug
{
    public class PushNotificationDebug : AbstractController
    {
        [Post]
        public void Send(uint[] targetPlayerIds, string message)
        {
            PushNotificationDebugService.Send(targetPlayerIds, message);
        }
    }
}
