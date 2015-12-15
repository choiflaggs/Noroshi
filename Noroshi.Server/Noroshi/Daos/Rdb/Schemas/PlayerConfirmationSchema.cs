namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerConfirmationSchema
	{
		public static string TableName => "player_confirmation";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte ReferenceID { get; set; }
			public System.UInt32 ConfirmedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte ReferenceID { get; set; }
		}
	}
}
