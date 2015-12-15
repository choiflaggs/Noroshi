namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildRankRewardSchema
	{
		public static string TableName => "guild_rank_reward";

		public class Record : AbstractRecord
		{
			public System.Byte GuildRank { get; set; }
			public System.Byte No { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.Byte GuildRank { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
