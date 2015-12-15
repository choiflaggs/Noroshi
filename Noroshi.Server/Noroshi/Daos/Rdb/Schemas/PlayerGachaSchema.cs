namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerGachaSchema
	{
		public static string TableName => "player_gacha";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GachaID { get; set; }
			public System.UInt32 HitNum { get; set; }
			public System.UInt32 MissLotNum { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GachaID { get; set; }
		}
	}
}
