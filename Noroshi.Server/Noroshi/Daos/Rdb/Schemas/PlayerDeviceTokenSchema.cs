namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerDeviceTokenSchema
	{
		public static string TableName => "player_device_token";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte Type { get; set; }
			public System.Byte Status { get; set; }
			public System.String Token { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte Type { get; set; }
		}
	}
}
