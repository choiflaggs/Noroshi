namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TimeDebugSchema
	{
		public static string TableName => "time_debug";

		public class Record : AbstractRecord
		{
			public System.String HostName { get; set; }
			public System.Int32 ModifyTime { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.String HostName { get; set; }
		}
	}
}
