namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GooseDbVersionSchema
	{
		public static string TableName => "goose_db_version";

		public class Record : AbstractRecord
		{
			public System.UInt64 id { get; set; }
			public System.Int64 version_id { get; set; }
			public System.SByte is_applied { get; set; }
			public System.DateTime tstamp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt64 id { get; set; }
		}
	}
}
