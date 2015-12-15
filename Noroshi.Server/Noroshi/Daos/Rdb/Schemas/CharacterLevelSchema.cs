namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterLevelSchema
	{
		public static string TableName => "character_level";

		public class Record : AbstractRecord
		{
			public System.UInt16 Level { get; set; }
			public System.UInt32 Exp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt16 Level { get; set; }
		}
	}
}
