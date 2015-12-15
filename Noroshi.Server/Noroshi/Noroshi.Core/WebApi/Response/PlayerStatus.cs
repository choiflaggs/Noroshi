using Noroshi.Core.Game.Guild;

namespace Noroshi.Core.WebApi.Response
{
    public class PlayerStatus
    {
        public uint PlayerID { get; set; }
        public uint LanguageID { get; set; }
        public string Name { get; set; }
        public ushort Level { get; set; }
        public uint Exp { get; set; }
        public uint ExpInLevel { get; set; }
        public ushort VipLevel { get; set; }
        public uint VipExp { get; set; }
        public uint VipExpInLevel { get; set; }
        public ushort LastStamina { get; set; }
        public uint LastStaminaUpdatedAt { get; set; }
        public ushort MaxStamina { get; set; }
        public uint LastStaminaRecoveryNum { get; set; }
        public uint LastStaminaRecoveredAt { get; set; }
        public uint LastGoldRecoveryNum { get; set; }
        public uint LastGoldRecoveredAt { get; set; }
        public ushort LastBP { get; set; }
        public uint LastBPUpdatedAt { get; set; }
        public ushort MaxBP { get; set; }
        public uint LastBPRecoveryNum { get; set; }
        public uint LastBPRecoveredAt { get; set; }
        public byte LastActionLevelPoint { get; set; }
        public uint LastActionLevelPointUpdatedAt { get; set; }
        public byte MaxActionLevelPoint { get; set; }
        public uint LastActionLevelPointRecoveryNum { get; set; }
        public uint LastActionLevelPointRecoveredAt { get; set; }
        public uint Gold { get; set; }
        public uint Gem { get; set; }
        public uint AvaterCharacterID { get; set; }
        /// <summary>
        /// 所属ギルド ID。
        /// </summary>
        public uint? GuildID { get; set; }
        /// <summary>
        /// 所属ギルドにおける役割。
        /// </summary>
        public GuildRole? GuildRole { get; set; }
        public ushort TutorialStep { get; set; }
    }
}
