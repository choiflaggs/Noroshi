namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerVipLevelBonusSchema
	{
		public static string TableName => "player_vip_level_bonus";

		public class Record : AbstractRecord
		{
			public System.UInt16 Level { get; set; }
			public System.Byte QuickBattle { get; set; }
			public System.Byte ConsecutiveQuickBattles { get; set; }
			public System.Byte GemToGearEnchant { get; set; }
			public System.Byte VipGacha { get; set; }
			public System.Byte MaxStaminaRecoveryNum { get; set; }
			public System.Byte MaxBackStoryRecoveryNum { get; set; }
			public System.Byte MaxActionLevelPointRecoveryNum { get; set; }
			public System.Byte MaxRentalCharacterNum { get; set; }
			public System.Byte MaxResetExpeditionNum { get; set; }
			public System.Byte MaxGreetingNum { get; set; }
			public System.Single GemBonus { get; set; }
			public System.Single GuildPointBonus { get; set; }
			public System.Single ExpeditionPointBonus { get; set; }
			public System.Single ArenaPointBonus { get; set; }
			public System.Single ExpeditionGoldBonus { get; set; }
			public System.UInt16 DailyReceivableRaidTicketNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt16 Level { get; set; }
		}
	}
}
