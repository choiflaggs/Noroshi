namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class QuestSchema
	{
		public static string TableName => "quest";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.Byte TriggerID { get; set; }
			public System.UInt32 Threshold { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
