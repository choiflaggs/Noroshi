namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ShopMerchandiseSchema
	{
		public static string TableName => "shop_merchandise";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 MerchandiseGroupID { get; set; }
			public System.UInt16 MinPlayerLevel { get; set; }
			public System.UInt16 MaxPlayerLevel { get; set; }
			public System.Byte MerchandisePossessionCategory { get; set; }
			public System.UInt32 MerchandisePossessionID { get; set; }
			public System.UInt32 MerchandisePossessionNum { get; set; }
			public System.Byte PaymentPossessionCategory { get; set; }
			public System.UInt32 PaymentPossessionID { get; set; }
			public System.UInt32 PaymentPossessionNum { get; set; }
			public System.UInt32 Weight { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
