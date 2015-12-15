namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ExchangeCashGiftSchema
	{
		public static string TableName => "exchange_cash_gift";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 Gold { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
