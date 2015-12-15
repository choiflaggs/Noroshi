namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerTrialStageSchema
	{
		public static string TableName => "player_trial_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrialStageID { get; set; }
			public System.Byte Rank { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrialStageID { get; set; }
		}
	}
}
