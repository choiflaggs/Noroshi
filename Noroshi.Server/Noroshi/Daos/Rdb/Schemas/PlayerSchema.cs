namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerSchema
	{
		public static string TableName => "player";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String UDID { get; set; }
			public System.String SessionID { get; set; }
			public System.UInt32 ShardID { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
