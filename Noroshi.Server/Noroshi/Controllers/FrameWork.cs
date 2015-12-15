using LightNode.Server;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services;
using Noroshi.Core.WebApi.Response.FrameWork;

namespace Noroshi.Server.Controllers
{
    public class FrameWork : AbstractController
    {
        [OperationOption(AcceptVerbs.Get, typeof(LightNode.Formatter.DataContractJsonContentFormatterFactory))]
        public CheckAliveResponse CheckAlive()
        {
            return new CheckAliveResponse();
        }

    }
}