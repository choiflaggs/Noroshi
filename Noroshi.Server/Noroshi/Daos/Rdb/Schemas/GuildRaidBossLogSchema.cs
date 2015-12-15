namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildRaidBossLogSchema
	{
		public static string TableName => "guild_raid_boss_log";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GuildRaidBossID { get; set; }
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 Damage { get; set; }
			public System.UInt32 GuildRaidBossCreatedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GuildRaidBossCreatedAt { get; set; }
		}
	}
}
