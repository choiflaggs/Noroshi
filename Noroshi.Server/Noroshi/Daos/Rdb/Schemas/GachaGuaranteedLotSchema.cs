namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GachaGuaranteedLotSchema
	{
		public static string TableName => "gacha_guaranteed_lot";

		public class Record : AbstractRecord
		{
			public System.UInt32 GachaID { get; set; }
			public System.UInt32 HitNum { get; set; }
			public System.UInt32 MissLotNum { get; set; }
			public System.Byte GuaranteedPossessionCategory { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 GachaID { get; set; }
			public System.UInt32 HitNum { get; set; }
		}
	}
}
