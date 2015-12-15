namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GearPieceSchema
	{
		public static string TableName => "gear_piece";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 EnchantExp { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
