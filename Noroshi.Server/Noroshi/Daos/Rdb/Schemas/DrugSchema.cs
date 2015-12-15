namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class DrugSchema
	{
		public static string TableName => "drug";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CharacterExp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
