using LightNode.Server;
using Noroshi.Server.Services.Debug;

namespace Noroshi.Server.Controllers.Debug
{
    public class TimeDebug : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Debug.TimeDebug Get() => TimeDebugService.Get();

        [Post]
        public Core.WebApi.Response.Debug.TimeDebug ChangeTime(int year, int month, int day, int hour, int minute, int second)
            => TimeDebugService.ChangeTime(year, month, day, hour, minute, second);
    }
}