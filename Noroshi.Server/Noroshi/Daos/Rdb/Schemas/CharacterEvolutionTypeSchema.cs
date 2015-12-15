namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterEvolutionTypeSchema
	{
		public static string TableName => "character_evolution_type";

		public class Record : AbstractRecord
		{
			public System.UInt16 Type { get; set; }
			public System.Byte EvolutionLevel { get; set; }
			public System.UInt16 Soul { get; set; }
			public System.UInt32 NecessaryGold { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt16 Type { get; set; }
			public System.Byte EvolutionLevel { get; set; }
		}
	}
}
