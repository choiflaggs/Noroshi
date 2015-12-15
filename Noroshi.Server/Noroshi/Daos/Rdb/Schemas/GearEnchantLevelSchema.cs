namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GearEnchantLevelSchema
	{
		public static string TableName => "gear_enchant_level";

		public class Record : AbstractRecord
		{
			public System.UInt32 GearID { get; set; }
			public System.Byte EnchantLevel { get; set; }
			public System.Single GrowthRate { get; set; }
			public System.UInt32 Exp { get; set; }
			public System.UInt32 Gold { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 GearID { get; set; }
			public System.Byte EnchantLevel { get; set; }
		}
	}
}
