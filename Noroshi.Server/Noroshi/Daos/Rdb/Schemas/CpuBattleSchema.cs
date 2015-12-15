namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CpuBattleSchema
	{
		public static string TableName => "cpu_battle";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 FieldID { get; set; }
			public System.UInt32 CharacterExp { get; set; }
			public System.UInt32 Gold { get; set; }
			public System.UInt32 GachaID { get; set; }
			public System.Byte MinDropNum { get; set; }
			public System.Byte MaxDropNum { get; set; }
			public System.UInt32 BossCpuCharacterID { get; set; }
			public System.Single BossSoulDropRatio { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
