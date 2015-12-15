namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerLevelSchema
	{
		public static string TableName => "player_level";

		public class Record : AbstractRecord
		{
			public System.UInt16 Level { get; set; }
			public System.UInt32 Exp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt16 Level { get; set; }
		}
	}
}
