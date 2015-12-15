namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class RankingRewardSchema
	{
		public static string TableName => "ranking_reward";

		public class Record : AbstractRecord
		{
			public System.UInt32 RankingID { get; set; }
			public System.UInt32 ThresholdRank { get; set; }
			public System.Byte No { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 RankingID { get; set; }
			public System.UInt32 ThresholdRank { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
