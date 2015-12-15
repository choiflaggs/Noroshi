namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerTrainingStageSchema
	{
		public static string TableName => "player_training_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrainingStageID { get; set; }
			public System.UInt32 Score { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrainingStageID { get; set; }
		}
	}
}
