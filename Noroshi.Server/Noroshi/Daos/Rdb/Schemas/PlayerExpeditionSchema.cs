namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerExpeditionSchema
	{
		public static string TableName => "player_expedition";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte ClearLevel { get; set; }
			public System.Byte LastResetNum { get; set; }
			public System.UInt32 LastResetedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
