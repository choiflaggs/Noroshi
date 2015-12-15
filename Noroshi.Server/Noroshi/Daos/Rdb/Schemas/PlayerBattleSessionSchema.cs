namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerBattleSessionSchema
	{
		public static string TableName => "player_battle_session";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.String SessionID { get; set; }
			public System.String PreprocessData { get; set; }
			public System.UInt32 PlayerCharacterID1 { get; set; }
			public System.UInt32 PlayerCharacterID2 { get; set; }
			public System.UInt32 PlayerCharacterID3 { get; set; }
			public System.UInt32 PlayerCharacterID4 { get; set; }
			public System.UInt32 PlayerCharacterID5 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
