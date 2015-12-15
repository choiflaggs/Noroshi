namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GachaEntryPointSchema
	{
		public static string TableName => "gacha_entry_point";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.UInt32 GameContentID { get; set; }
			public System.UInt32 OpenedAt { get; set; }
			public System.UInt32 ClosedAt { get; set; }
			public System.UInt32 GachaID { get; set; }
			public System.Byte LotNum { get; set; }
			public System.Byte MaxTotalLotNum { get; set; }
			public System.Byte MaxDailyFreeLotNum { get; set; }
			public System.UInt16 FreeLotCoolTimeMinute { get; set; }
			public System.Byte PaymentPossessionCategory { get; set; }
			public System.UInt32 PaymentPossessionID { get; set; }
			public System.UInt32 PaymentPossessionNum { get; set; }
			public System.Byte GuaranteedPossessionCategory { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
