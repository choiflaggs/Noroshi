using Noroshi.Core.Game.Sound;

namespace Noroshi.Core.WebApi.Response.Master
{
    public class Sound
    {
        public uint ID { get; set; }
        public string Path { get; set; }
        public byte ChannelNum { get; set; }
        public PlayType PlayType { get; set; }
    }
}
