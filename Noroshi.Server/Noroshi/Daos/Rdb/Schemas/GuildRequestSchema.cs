namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildRequestSchema
	{
		public static string TableName => "guild_request";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 GuildID { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
