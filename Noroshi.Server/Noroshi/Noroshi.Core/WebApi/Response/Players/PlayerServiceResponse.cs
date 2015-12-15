using Noroshi.Core.WebApi.Response;

namespace Noroshi.Core.WebApi.Response.Players
{
    public class PlayerServiceResponse
    {
        public PlayerError Error { get; set; }
        public PlayerStatus PlayerStatus { get; set; }
    }
}