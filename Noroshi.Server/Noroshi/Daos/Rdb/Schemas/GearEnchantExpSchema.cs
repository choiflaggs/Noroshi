namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GearEnchantExpSchema
	{
		public static string TableName => "gear_enchant_exp";

		public class Record : AbstractRecord
		{
			public System.UInt32 GearID { get; set; }
			public System.UInt32 EnchantExp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 GearID { get; set; }
		}
	}
}
