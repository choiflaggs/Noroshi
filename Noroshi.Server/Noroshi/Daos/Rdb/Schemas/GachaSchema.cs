namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GachaSchema
	{
		public static string TableName => "gacha";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
