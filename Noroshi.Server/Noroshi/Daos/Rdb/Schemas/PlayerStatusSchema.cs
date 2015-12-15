namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerStatusSchema
	{
		public static string TableName => "player_status";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 LanguageID { get; set; }
			public System.String Name { get; set; }
			public System.UInt32 Exp { get; set; }
			public System.UInt32 VipExp { get; set; }
			public System.UInt32 GuildID { get; set; }
			public System.Byte GuildRole { get; set; }
			public System.Byte GuildState { get; set; }
			public System.UInt32 BeginnerGuildDroppedOutOn { get; set; }
			public System.Byte LastGreetingNum { get; set; }
			public System.UInt32 LastGreetedAt { get; set; }
			public System.UInt32 UnconfirmedGreetedNum { get; set; }
			public System.Byte LastRentalRewardReceivingNum { get; set; }
			public System.UInt32 LastRentalRewardReceivedAt { get; set; }
			public System.Byte LastBP { get; set; }
			public System.UInt32 LastBPUpdatedAt { get; set; }
			public System.UInt32 LastBPRecoveryNum { get; set; }
			public System.UInt32 LastBPRecoveredAt { get; set; }
			public System.UInt16 LastStamina { get; set; }
			public System.UInt32 LastStaminaUpdatedAt { get; set; }
			public System.UInt32 LastStaminaRecoveryNum { get; set; }
			public System.UInt32 LastStaminaRecoveredAt { get; set; }
			public System.UInt32 LastGoldRecoveryNum { get; set; }
			public System.UInt32 LastGoldRecoveredAt { get; set; }
			public System.Byte LastActionLevelPoint { get; set; }
			public System.UInt32 LastActionLevelPointUpdatedAt { get; set; }
			public System.UInt32 LastActionLevelPointRecoveryNum { get; set; }
			public System.UInt32 LastActionLevelPointRecoveredAt { get; set; }
			public System.UInt32 Gold { get; set; }
			public System.UInt32 FreeGem { get; set; }
			public System.UInt32 ChargeGem { get; set; }
			public System.UInt32 ArenaPoint { get; set; }
			public System.UInt32 GuildPoint { get; set; }
			public System.UInt32 ExpeditionPoint { get; set; }
			public System.UInt32 SoulPoint { get; set; }
			public System.UInt32 AvaterCharacterID { get; set; }
			public System.UInt16 TutorialStep { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
