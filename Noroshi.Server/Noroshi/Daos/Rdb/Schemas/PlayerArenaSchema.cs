namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerArenaSchema
	{
		public static string TableName => "player_arena";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 Rank { get; set; }
			public System.UInt32 BestRank { get; set; }
			public System.UInt32 DeckPlayerCharacterID1 { get; set; }
			public System.UInt32 DeckPlayerCharacterID2 { get; set; }
			public System.UInt32 DeckPlayerCharacterID3 { get; set; }
			public System.UInt32 DeckPlayerCharacterID4 { get; set; }
			public System.UInt32 DeckPlayerCharacterID5 { get; set; }
			public System.UInt32 Win { get; set; }
			public System.UInt32 Lose { get; set; }
			public System.UInt32 DefenseWin { get; set; }
			public System.UInt32 DefenseLose { get; set; }
			public System.UInt32 AllHP { get; set; }
			public System.UInt32 AllStrength { get; set; }
			public System.UInt32 PlayNum { get; set; }
			public System.UInt32 BattleStartedAt { get; set; }
			public System.UInt32 LastBattledAt { get; set; }
			public System.UInt32 PlayResetNum { get; set; }
			public System.UInt32 LastPlayResetAt { get; set; }
			public System.UInt32 CoolTimeAt { get; set; }
			public System.UInt32 CoolTimeResetNum { get; set; }
			public System.UInt32 LastCoolTimeResetAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
