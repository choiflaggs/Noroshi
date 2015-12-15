using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Core.WebApi.Response.Arena
{
    public class ArenaServiceResponse
    {
        public PlayerError Error { get; set; }
        public PlayerStatus PlayerStatus { get; set; }
        public PlayerArena PlayerArena { get; set; }
    }
}