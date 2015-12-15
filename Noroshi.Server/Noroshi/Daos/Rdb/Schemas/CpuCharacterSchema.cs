namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CpuCharacterSchema
	{
		public static string TableName => "cpu_character";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CharacterID { get; set; }
			public System.UInt16 Level { get; set; }
			public System.Byte PromotionLevel { get; set; }
			public System.Byte EvolutionLevel { get; set; }
			public System.UInt32 FixedMaxHP { get; set; }
			public System.UInt16 InitialEnergy { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
