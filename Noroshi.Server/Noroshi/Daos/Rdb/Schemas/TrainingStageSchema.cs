namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class TrainingStageSchema
	{
		public static string TableName => "training_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 TrainingID { get; set; }
			public System.Byte Level { get; set; }
			public System.Single CharacterExpCoefficient { get; set; }
			public System.Single GoldCoefficient { get; set; }
			public System.UInt16 NecessaryPlayerLevel { get; set; }
			public System.UInt32 CpuBattleID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
