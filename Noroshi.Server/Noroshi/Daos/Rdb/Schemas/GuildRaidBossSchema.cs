namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildRaidBossSchema
	{
		public static string TableName => "guild_raid_boss";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GuildID { get; set; }
			public System.UInt32 RaidBossID { get; set; }
			public System.Byte State { get; set; }
			public System.UInt32 DiscoveryPlayerID { get; set; }
			public System.UInt32 LastBattlePlayerID { get; set; }
			public System.UInt32 ComboNum { get; set; }
			public System.String BattleData { get; set; }
			public System.String EntryData { get; set; }
			public System.UInt32 DefeatedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
