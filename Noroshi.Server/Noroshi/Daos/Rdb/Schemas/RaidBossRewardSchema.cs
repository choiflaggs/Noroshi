namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class RaidBossRewardSchema
	{
		public static string TableName => "raid_boss_reward";

		public class Record : AbstractRecord
		{
			public System.UInt32 RaidBossID { get; set; }
			public System.Byte Category { get; set; }
			public System.Byte No { get; set; }
			public System.Single Probability { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 RaidBossID { get; set; }
			public System.Byte Category { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
