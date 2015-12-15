namespace Noroshi.Core.WebApi.Response.Story
{
    public class PlayerStoryStage
    {
        public uint StageID { get; set; }
        public byte Rank { get; set; }
        public uint PlayCount { get; set; }
        public StoryStage Stage { get; set; }
    }
}