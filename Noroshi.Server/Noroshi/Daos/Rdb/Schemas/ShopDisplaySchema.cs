namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ShopDisplaySchema
	{
		public static string TableName => "shop_display";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 ShopID { get; set; }
			public System.Byte No { get; set; }
			public System.UInt32 MerchandiseGroupID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
