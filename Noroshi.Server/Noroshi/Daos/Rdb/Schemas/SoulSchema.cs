namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class SoulSchema
	{
		public static string TableName => "soul";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CharacterID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
