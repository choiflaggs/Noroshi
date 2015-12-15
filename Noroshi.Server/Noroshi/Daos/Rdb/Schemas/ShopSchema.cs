namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ShopSchema
	{
		public static string TableName => "shop";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.Byte PaymentPossessionCategory { get; set; }
			public System.UInt32 PaymentPossessionID { get; set; }
			public System.Byte ManualUpdatePossessionCategory { get; set; }
			public System.UInt32 ManualUpdatePossessionID { get; set; }
			public System.UInt32 ManualUpdatePossessionNum { get; set; }
			public System.Byte DailyScheduledUpdateNum { get; set; }
			public System.UInt32 RelatedGameContentID { get; set; }
			public System.UInt16 AppearPlayerLevel { get; set; }
			public System.Single AppearProbability { get; set; }
			public System.UInt16 AppearMinute { get; set; }
			public System.UInt16 ResidentVipLevel { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
