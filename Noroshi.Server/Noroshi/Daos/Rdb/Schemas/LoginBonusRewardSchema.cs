namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class LoginBonusRewardSchema
	{
		public static string TableName => "login_bonus_reward";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 LoginBonusID { get; set; }
			public System.Byte Threshold { get; set; }
			public System.Byte PossessionCategory { get; set; }
			public System.UInt32 PossessionID { get; set; }
			public System.UInt32 PossessionNum { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
