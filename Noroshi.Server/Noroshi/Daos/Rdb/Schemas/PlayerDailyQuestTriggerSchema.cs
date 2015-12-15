namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerDailyQuestTriggerSchema
	{
		public static string TableName => "player_daily_quest_trigger";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TriggerID { get; set; }
			public System.UInt32 CurrentNum { get; set; }
			public System.UInt32 ReceiveRewardThreshold { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TriggerID { get; set; }
		}
	}
}
