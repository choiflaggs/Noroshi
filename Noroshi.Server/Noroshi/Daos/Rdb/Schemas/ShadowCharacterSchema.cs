namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ShadowCharacterSchema
	{
		public static string TableName => "shadow_character";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CharacterID { get; set; }
			public System.UInt16 Level { get; set; }
			public System.Byte PromotionLevel { get; set; }
			public System.Byte EvolutionLevel { get; set; }
			public System.UInt16 ActionLevel1 { get; set; }
			public System.UInt16 ActionLevel2 { get; set; }
			public System.UInt16 ActionLevel3 { get; set; }
			public System.UInt16 ActionLevel4 { get; set; }
			public System.UInt16 ActionLevel5 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
