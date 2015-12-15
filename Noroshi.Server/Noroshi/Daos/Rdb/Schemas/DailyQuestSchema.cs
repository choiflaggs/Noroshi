namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class DailyQuestSchema
	{
		public static string TableName => "daily_quest";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.Byte TriggerID { get; set; }
			public System.UInt32 Threshold { get; set; }
			public System.UInt32 OpenHour { get; set; }
			public System.UInt32 CloseHour { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
