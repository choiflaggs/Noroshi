namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerTrainingSchema
	{
		public static string TableName => "player_training";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrainingID { get; set; }
			public System.Byte LastBattleNum { get; set; }
			public System.UInt32 LastBattledAt { get; set; }
			public System.UInt32 ReopenedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TrainingID { get; set; }
		}
	}
}
