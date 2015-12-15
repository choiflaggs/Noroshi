namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TrainingDateSchema
	{
		public static string TableName => "training_date";

		public class Record : AbstractRecord
		{
			public System.UInt32 TrainingID { get; set; }
			public System.Byte Day { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 TrainingID { get; set; }
			public System.Byte Day { get; set; }
		}
	}
}
