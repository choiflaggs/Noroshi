using Noroshi.Core.Game.Guild;

namespace Noroshi.Core.WebApi.Response
{
    public class OtherPlayerStatus
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public ushort Level { get; set; }
        public uint AvaterCharacterID { get; set; }
        /// <summary>
        /// 所属ギルド ID。
        /// </summary>
        public uint? GuildID { get; set; }
        /// <summary>
        /// 所属ギルドにおける役割。
        /// </summary>
        public GuildRole? GuildRole { get; set; }
    }
}
