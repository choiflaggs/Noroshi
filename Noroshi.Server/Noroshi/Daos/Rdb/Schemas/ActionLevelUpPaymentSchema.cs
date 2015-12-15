namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ActionLevelUpPaymentSchema
	{
		public static string TableName => "action_level_up_payment";

		public class Record : AbstractRecord
		{
			public System.UInt16 Level { get; set; }
			public System.UInt32 Gold { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt16 Level { get; set; }
		}
	}
}
