namespace Noroshi.Core.WebApi.Response.Guild
{
    public class CreateResponse
    {
        public GuildError Error { get; set; }
        public Guild NewGuild { get; set; }
    }
}
