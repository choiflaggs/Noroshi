namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerShopSchema
	{
		public static string TableName => "player_shop";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 ShopID { get; set; }
			public System.String Merchandises { get; set; }
			public System.String BoughtDisplayNos { get; set; }
			public System.UInt16 MerchandiseManualUpdateNum { get; set; }
			public System.UInt32 MerchandiseUpdatedAtManually { get; set; }
			public System.UInt32 MerchandiseUpdatedAtOnSchedule { get; set; }
			public System.UInt32 AppearedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 ShopID { get; set; }
		}
	}
}
