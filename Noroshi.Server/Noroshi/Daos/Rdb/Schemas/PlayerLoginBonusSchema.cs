namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerLoginBonusSchema
	{
		public static string TableName => "player_login_bonus";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 LoginBonusID { get; set; }
			public System.Byte CurrentNum { get; set; }
			public System.Byte ReceiveRewardThreshold { get; set; }
			public System.UInt32 LastLoggedInAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 LoginBonusID { get; set; }
		}
	}
}
