namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class InformationSchema
	{
		public static string TableName => "information";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.String BannerInformation { get; set; }
			public System.Byte Category { get; set; }
			public System.UInt32 OpenedAt { get; set; }
			public System.UInt32 ClosedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
