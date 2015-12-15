namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerActivityDailyLogSchema
	{
		public static string TableName => "player_activity_daily_log";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 StaminaConsuming { get; set; }
			public System.UInt32 BPConsuming { get; set; }
			public System.UInt32 CreatedOn { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 CreatedOn { get; set; }
		}
	}
}
