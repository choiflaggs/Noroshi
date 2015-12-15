namespace Noroshi.Core.WebApi.Response
{
    public class PlayerStatusLevelUpResponse
    {
        public PlayerChangeStatusResponse BeforePlayerStatus
        { get; set; }
        public PlayerChangeStatusResponse AfterPlayerStatus
        { get; set; }
        public ContentsUnlockResponse[] ImportantContentsUnlocks
        { get; set; }
        public ContentsUnlockResponse[] ContentsUnlocks
        { get; set; }
    }
}