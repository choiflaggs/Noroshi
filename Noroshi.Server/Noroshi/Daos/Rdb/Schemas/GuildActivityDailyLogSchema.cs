namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildActivityDailyLogSchema
	{
		public static string TableName => "guild_activity_daily_log";

		public class Record : AbstractRecord
		{
			public System.UInt32 GuildID { get; set; }
			public System.UInt32 BPConsuming { get; set; }
			public System.UInt32 DefeatRaidBossNum { get; set; }
			public System.UInt16 CooperationPoint { get; set; }
			public System.UInt32 CreatedOn { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 GuildID { get; set; }
			public System.UInt32 CreatedOn { get; set; }
		}
	}
}
