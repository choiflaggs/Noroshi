
namespace Noroshi.Core.WebApi.Response.Guild
{
    public class GuildMemberPlayerStatus : OtherPlayerStatus
    {
        /// <summary>
        /// 挨拶できるかどうか。
        /// </summary>
        public bool CanGreet { get; set; }        
    }
}
