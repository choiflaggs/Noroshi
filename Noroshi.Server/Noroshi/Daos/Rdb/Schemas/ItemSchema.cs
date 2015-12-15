namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ItemSchema
	{
		public static string TableName => "item";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.Byte Type { get; set; }
			public System.Byte Rarity { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
