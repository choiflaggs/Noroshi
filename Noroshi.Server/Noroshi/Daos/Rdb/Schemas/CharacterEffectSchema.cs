namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterEffectSchema
	{
		public static string TableName => "character_effect";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 PrefabID { get; set; }
			public System.String AnimationName { get; set; }
			public System.Byte MultiAnimation { get; set; }
			public System.SByte HasText { get; set; }
			public System.Int16 OrderInCharacterLayer { get; set; }
			public System.Byte Position { get; set; }
			public System.Byte FixedRotationY { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
