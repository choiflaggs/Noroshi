namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TrialStageSchema
	{
		public static string TableName => "trial_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 TrialID { get; set; }
			public System.Byte Level { get; set; }
			public System.UInt32 CpuBattleID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
