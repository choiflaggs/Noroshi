namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GearRecipeSchema
	{
		public static string TableName => "gear_recipe";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 MaterialItemID { get; set; }
			public System.UInt32 CraftItemID { get; set; }
			public System.Byte MaterialType { get; set; }
			public System.UInt32 Count { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
