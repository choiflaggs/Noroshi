namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class DailyQuestRewardSchema
	{
		public static string TableName => "daily_quest_reward";

		public class Record : AbstractRecord
		{
			public System.UInt32 DailyQuestID { get; set; }
			public System.Byte No { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 DailyQuestID { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
