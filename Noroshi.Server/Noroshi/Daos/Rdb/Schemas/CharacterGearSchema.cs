namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterGearSchema
	{
		public static string TableName => "character_gear";

		public class Record : AbstractRecord
		{
			public System.UInt32 CharacterID { get; set; }
			public System.Byte PromotionLevel { get; set; }
			public System.UInt32 GearID1 { get; set; }
			public System.UInt32 GearID2 { get; set; }
			public System.UInt32 GearID3 { get; set; }
			public System.UInt32 GearID4 { get; set; }
			public System.UInt32 GearID5 { get; set; }
			public System.UInt32 GearID6 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 CharacterID { get; set; }
			public System.Byte PromotionLevel { get; set; }
		}
	}
}
