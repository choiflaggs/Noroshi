namespace Noroshi.Core.WebApi.Response
{
    public class PlayerVipLevelBonus
    {
        public ushort Level { get; set; }
        public bool QuickBattle { get; set; }
        public bool ConsecutiveQuickBattles { get; set; }
        public bool GemToGearEnchant { get; set; }
        public StandingRaidShop[] StandingRaidShops { get; set; }
        public bool VipGacha { get; set; }
        public byte MaxActionLevelPointRecoveryNum { get; set; }
        public byte MaxStaminaRecoveryNum { get; set; }
        public byte MaxBackStoryRecoveryNum { get; set; }
        public byte MaxRentalCharacterNum { get; set; }
        public byte MaxResetExpeditionNum { get; set; }
        public byte MaxGreetingNum { get; set; }
        public float GemBonus { get; set; }
        public float GuildPointBonus { get; set; }
        public float ExpeditionPointBonus { get; set; }
        public float ArenaPointBonus { get; set; }
        public float ExpeditionGoldBonus { get; set; }
        public ushort DailyReceivableRaidTicketNum { get; set; }
    }
}