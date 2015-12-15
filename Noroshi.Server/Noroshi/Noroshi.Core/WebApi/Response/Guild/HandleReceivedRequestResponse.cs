namespace Noroshi.Core.WebApi.Response.Guild
{
    public class HandleReceivedRequestResponse
    {
        public OtherPlayerStatus Requester { get; set; }
        public GuildError Error { get; set; }
    }
}
