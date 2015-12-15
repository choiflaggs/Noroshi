using System.Linq;
using LightNode.Server;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Controllers
{
    public class StoryEpisode : AbstractController
    {
        [Get]
        public Core.WebApi.Response.Story.StoryEpisode[] MasterData() => StoryEpisodeEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
    }
}