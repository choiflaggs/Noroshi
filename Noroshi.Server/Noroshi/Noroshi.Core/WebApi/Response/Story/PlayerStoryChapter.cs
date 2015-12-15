namespace Noroshi.Core.WebApi.Response.Story
{
    public class PlayerStoryChapter
    {
        public uint ChapterID { get; set; }
        public PlayerStoryEpisode[] PlayerEpisodeList { get; set; }
    }
}