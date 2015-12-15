using System;

namespace Noroshi.Datas
{
    public class PlayerStatus : PlayerData
    {
        public uint Language { get; set; }
        public string Name { get; set; }
        public uint Exp { get; set; }
        public uint VipExp { get; set; }
        public uint GuildID { get; set; }
        public DateTime StaminaTime { get; set; }
        public uint Gold { get; set; }
        public uint FreeGem { get; set; }
        public uint ChargeGem { get; set; }
        public uint AvaterCharacterID { get; set; }
    }
}