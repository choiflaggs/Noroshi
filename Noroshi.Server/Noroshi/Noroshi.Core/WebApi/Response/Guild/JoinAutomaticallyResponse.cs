namespace Noroshi.Core.WebApi.Response.Guild
{
    public class JoinAutomaticallyResponse
    {
        public GuildError Error { get; set; }
        public Guild NewGuild { get; set; }
    }
}
