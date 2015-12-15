namespace Noroshi.Core.WebApi.Response.Guild
{
    public class JoinResponse
    {
        public GuildError Error { get; set; }
        public Guild NewGuild { get; set; }
    }
}
