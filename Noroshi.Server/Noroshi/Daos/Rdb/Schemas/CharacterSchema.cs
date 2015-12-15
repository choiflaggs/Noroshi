namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterSchema
	{
		public static string TableName => "character";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.String TagFlags { get; set; }
			public System.Byte InitialEvolutionLevel { get; set; }
			public System.Byte Position { get; set; }
			public System.UInt32 OrderPriority { get; set; }
			public System.UInt32 OrderInLayer { get; set; }
			public System.Byte Type { get; set; }
			public System.UInt16 EvolutionType { get; set; }
			public System.Single Strength { get; set; }
			public System.Single Intellect { get; set; }
			public System.Single Agility { get; set; }
			public System.Single StrengthGrowth { get; set; }
			public System.Single IntellectGrowth { get; set; }
			public System.Single AgilityGrowth { get; set; }
			public System.UInt32 MagicCrit { get; set; }
			public System.UInt32 ArmorPenetration { get; set; }
			public System.UInt32 IgnoreMagicResistance { get; set; }
			public System.Byte Accuracy { get; set; }
			public System.Byte Dodge { get; set; }
			public System.UInt32 HPRegen { get; set; }
			public System.UInt16 EnergyRegen { get; set; }
			public System.Byte ImproveHealings { get; set; }
			public System.Byte ReduceEnergyCost { get; set; }
			public System.UInt32 LifeStealRating { get; set; }
			public System.UInt32 ActionID0 { get; set; }
			public System.UInt32 ActionID1 { get; set; }
			public System.UInt32 ActionID2 { get; set; }
			public System.UInt32 ActionID3 { get; set; }
			public System.UInt32 ActionID4 { get; set; }
			public System.UInt32 ActionID5 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
