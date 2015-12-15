namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class RaidBossSchema
	{
		public static string TableName => "raid_boss";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GroupID { get; set; }
			public System.Byte Level { get; set; }
			public System.UInt32 CpuBattleID { get; set; }
			public System.UInt32 GuildClusterID { get; set; }
			public System.Single EncounterProbability { get; set; }
			public System.UInt16 LifetimeMinute { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
