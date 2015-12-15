namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerCharacterGearSchema
	{
		public static string TableName => "player_character_gear";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 GearID { get; set; }
			public System.UInt32 PlayerCharacterID { get; set; }
			public System.Byte PromotionLevel { get; set; }
			public System.Byte GearPosition { get; set; }
			public System.UInt32 Exp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
