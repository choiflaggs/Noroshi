using Noroshi.Core.Game.Guild;
using Noroshi.Core.Game.Player;

namespace Noroshi.Core.WebApi.Response.Guild
{
    public class GuildChatMessage
    {
        public OtherPlayerStatus OtherPlayerStatus { get; set; }

        public uint ID { get; set; }
        public string Message { get; set; }
        public uint CreatedAt { get; set; }
    }

}
