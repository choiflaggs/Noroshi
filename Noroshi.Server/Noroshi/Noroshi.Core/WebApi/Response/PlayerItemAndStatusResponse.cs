using Noroshi.Core.WebApi.Response;

namespace Noroshi.Core.WebApi.Response
{
    public class PlayerItemAndStatusResponse
    {
        public PlayerStatus PlayerStatus { get; set; }
        public PlayerItem PlayerItem { get; set; }
    }
}