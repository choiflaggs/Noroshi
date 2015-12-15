namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class QuestRewardSchema
	{
		public static string TableName => "quest_reward";

		public class Record : AbstractRecord
		{
			public System.UInt32 QuestID { get; set; }
			public System.Byte No { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 QuestID { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
