namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class DynamicTextSchema
	{
		public static string TableName => "dynamic_text";

		public class Record : AbstractRecord
		{
			public System.UInt32 LanguageID { get; set; }
			public System.String TextKey { get; set; }
			public System.String Text { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 LanguageID { get; set; }
			public System.String TextKey { get; set; }
		}
	}
}
