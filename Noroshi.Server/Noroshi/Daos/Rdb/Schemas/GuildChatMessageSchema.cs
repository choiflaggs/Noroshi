namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildChatMessageSchema
	{
		public static string TableName => "guild_chat_message";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GuildID { get; set; }
			public System.Byte Status { get; set; }
			public System.UInt32 PlayerID { get; set; }
			public System.String Message { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
	}
}
