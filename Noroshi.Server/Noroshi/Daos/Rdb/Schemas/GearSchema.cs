namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GearSchema
	{
		public static string TableName => "gear";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt16 Level { get; set; }
			public System.UInt32 HP { get; set; }
			public System.Single Strength { get; set; }
			public System.Single Intellect { get; set; }
			public System.Single Agility { get; set; }
			public System.Single StrengthGrowth { get; set; }
			public System.Single IntellectGrowth { get; set; }
			public System.Single AgilityGrowth { get; set; }
			public System.UInt32 PhysicalAttack { get; set; }
			public System.UInt32 MagicPower { get; set; }
			public System.UInt32 Armor { get; set; }
			public System.UInt32 MagicResistance { get; set; }
			public System.UInt32 PhysicalCrit { get; set; }
			public System.UInt32 MagicCrit { get; set; }
			public System.UInt32 ArmorPenetration { get; set; }
			public System.UInt32 IgnoreMagicResistance { get; set; }
			public System.Byte Accuracy { get; set; }
			public System.Single Dodge { get; set; }
			public System.Single HPRegen { get; set; }
			public System.Single EnergyRegen { get; set; }
			public System.Single ImproveHealings { get; set; }
			public System.Single LifeStealRating { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
