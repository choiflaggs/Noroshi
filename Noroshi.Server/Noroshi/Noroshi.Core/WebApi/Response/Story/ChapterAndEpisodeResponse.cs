using Noroshi.Core.WebApi.Response.Story;

namespace Noroshi.Core.WebApi.Response.Story
{
    public class StoryChapterAndStoryEpisodeResponse
    {
        public StoryChapter Chapter { get; set; }
        public PlayerStoryStageAndPlayerStoryEpisodeResponse[] Episodes { get; set; }
    }
}