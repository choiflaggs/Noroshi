namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ExpeditionSchema
	{
		public static string TableName => "expedition";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.Byte Level { get; set; }
			public System.Byte AutomaticRecovery { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
