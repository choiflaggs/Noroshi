using Noroshi.Core.Game.Guild;
using Noroshi.Core.Game.Player;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// ギルドチャット.
    /// </summary>
    public class GuildChatMessageResponse
    {
        public GuildChatMessage[]       Messages { get; set; }
        public GuildError               Error;

    }

}
