namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TrialDateSchema
	{
		public static string TableName => "trial_date";

		public class Record : AbstractRecord
		{
			public System.UInt32 TrialID { get; set; }
			public System.Byte Day { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 TrialID { get; set; }
			public System.Byte Day { get; set; }
		}
	}
}
