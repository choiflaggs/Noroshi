using System.Collections.Generic;

namespace Noroshi.Core.WebApi.Response
{
    public class Gear : Item
    {
        public ushort Level { get; set; }
        public uint HP { get; set; }
        public float StrengthGrowth { get; set; }
        public float IntellectGrowth { get; set; }
        public float AgilityGrowth { get; set; }
        public float Strength { get; set; }
        public float Intellect { get; set; }
        public float Agility { get; set; }
        public uint MagicCrit { get; set; }
        public float HPRegen { get; set; }
        public float EnergyRegen { get; set; }
        public float Dodge { get; set; }
        public uint ArmorPenetration { get; set; }
        public float LifeStealRating { get; set; }
        public float ImproveHealings { get; set; }
        public uint IgnoreMagicResistance { get; set; }
        public uint PhysicalAttack { get; set; }
        public uint MagicPower { get; set; }
        public uint Armor { get; set; }
        public uint MagicResistance { get; set; }
        public uint PhysicalCrit { get; set; }
        public byte Accuracy { get; set; }
        public byte MaxEnchantLevel { get; set; }
        public uint MaxEnchantExp { get; set; }
        public GearEnchantExp GearEnchantExp { get; set; }
        public GearEnchantLevel[] GearEnchantLevels { get; set; }
        public uint[] NecessaryEnchantExpMaps { get; set; }
    }
}