namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerCharacterSchema
	{
		public static string TableName => "player_character";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 CharacterID { get; set; }
			public System.UInt32 Level { get; set; }
			public System.UInt32 Exp { get; set; }
			public System.UInt32 PromotionLevel { get; set; }
			public System.UInt32 EvolutionLevel { get; set; }
			public System.UInt32 ActionLevel1 { get; set; }
			public System.UInt32 ActionLevel2 { get; set; }
			public System.UInt32 ActionLevel3 { get; set; }
			public System.UInt32 ActionLevel4 { get; set; }
			public System.UInt32 ActionLevel5 { get; set; }
			public System.UInt32 ActionLevel6 { get; set; }
			public System.UInt32 Gear1 { get; set; }
			public System.UInt32 Gear2 { get; set; }
			public System.UInt32 Gear3 { get; set; }
			public System.UInt32 Gear4 { get; set; }
			public System.UInt32 Gear5 { get; set; }
			public System.UInt32 Gear6 { get; set; }
			public System.DateTime CreatedAt { get; set; }
			public System.DateTime UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
