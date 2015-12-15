namespace Noroshi.Core.WebApi.Response.Story
{
    public class PlayerStoryEpisode
    {
        public uint EpisodeID { get; set; }
        public bool IsClearEpisode { get; set; }
        public PlayerStoryStage[] PlayerStageList { get; set; }
    }
}