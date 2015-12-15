namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ExpeditionStageSchema
	{
		public static string TableName => "expedition_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 ExpeditionID { get; set; }
			public System.Byte Step { get; set; }
			public System.UInt16 MaxPlayerLevel { get; set; }
			public System.UInt16 MinPlayerLevel { get; set; }
			public System.UInt32 Gold { get; set; }
			public System.UInt32 ExpeditionPoint { get; set; }
			public System.UInt32 GachaID { get; set; }
			public System.Byte GachaLotNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
