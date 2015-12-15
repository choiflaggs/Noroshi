namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerCounterSchema
	{
		public static string TableName => "player_counter";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 Count { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
