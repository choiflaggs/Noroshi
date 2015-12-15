namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerGuildRaidBossSchema
	{
		public static string TableName => "player_guild_raid_boss";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GuildRaidBossID { get; set; }
			public System.Byte State { get; set; }
			public System.UInt32 Damage { get; set; }
			public System.UInt32 GuildRaidBossCreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GuildRaidBossID { get; set; }
			public System.UInt32 GuildRaidBossCreatedAt { get; set; }
		}
	}
}
