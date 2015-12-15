namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CpuBattleStorySchema
	{
		public static string TableName => "cpu_battle_story";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 BattleID { get; set; }
			public System.Byte TriggerID { get; set; }
			public System.Byte DramaType { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
