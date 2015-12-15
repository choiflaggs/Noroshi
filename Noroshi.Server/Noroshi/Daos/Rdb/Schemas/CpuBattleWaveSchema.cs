namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CpuBattleWaveSchema
	{
		public static string TableName => "cpu_battle_wave";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 BattleID { get; set; }
			public System.Byte No { get; set; }
			public System.UInt32 CpuCharacterID1 { get; set; }
			public System.UInt32 CpuCharacterID2 { get; set; }
			public System.UInt32 CpuCharacterID3 { get; set; }
			public System.UInt32 CpuCharacterID4 { get; set; }
			public System.UInt32 CpuCharacterID5 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
