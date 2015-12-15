namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerRentalCharacterSchema
	{
		public static string TableName => "player_rental_character";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte No { get; set; }
			public System.UInt32 PlayerCharacterID { get; set; }
			public System.UInt32 RentalNum { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.Byte No { get; set; }
		}
	}
}
