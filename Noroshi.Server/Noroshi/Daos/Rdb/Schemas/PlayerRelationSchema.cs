namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerRelationSchema
	{
		public static string TableName => "player_relation";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TargetPlayerID { get; set; }
			public System.UInt32 LastGreetingAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 TargetPlayerID { get; set; }
		}
	}
}
