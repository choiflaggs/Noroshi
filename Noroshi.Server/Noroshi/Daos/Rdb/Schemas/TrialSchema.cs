namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TrialSchema
	{
		public static string TableName => "trial";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.String TagFlags { get; set; }
			public System.UInt32 OpenedAt { get; set; }
			public System.UInt32 ClosedAt { get; set; }
			public System.Byte Sunday { get; set; }
			public System.Byte Monday { get; set; }
			public System.Byte Tuesday { get; set; }
			public System.Byte Wednesday { get; set; }
			public System.Byte Thursday { get; set; }
			public System.Byte Friday { get; set; }
			public System.Byte Saturday { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
