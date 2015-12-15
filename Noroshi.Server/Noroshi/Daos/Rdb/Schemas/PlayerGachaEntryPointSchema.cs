namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerGachaEntryPointSchema
	{
		public static string TableName => "player_gacha_entry_point";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GachaEntryPointID { get; set; }
			public System.UInt32 TotalLotNum { get; set; }
			public System.UInt32 FreeReopenedAt { get; set; }
			public System.Byte LastFreeLotNum { get; set; }
			public System.UInt32 LastFreeLottedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GachaEntryPointID { get; set; }
		}
	}
}
