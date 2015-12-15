namespace Noroshi.Core.WebApi.Response.Character
{
    public class Character
    {
        public uint ID { get; set; }
        public string TagFlags { get; set; }
        public string TextKey { get; set; }
        public byte InitialEvolutionLevel { get; set; }
        public byte Position { get; set; }
        public uint OrderPriority { get; set; }
        public uint OrderInLayer { get; set; }
        public byte Type { get; set; }

        // MaxHP : 計算
        public float Strength { get; set; }
        public float Intellect { get; set; }
        public float Agility { get; set; }
        public float StrengthGrowth { get; set; }
        public float IntellectGrowth { get; set; }
        public float AgilityGrowth { get; set; }
        // PhysicalAttack : 計算
        // MagicPower : 計算
        // Armor : 計算
        // MagicRegistance : 計算
        // PhysicalCrit : 計算
        public uint MagicCrit { get; set; }
        public uint ArmorPenetration { get; set; }
        public uint IgnoreMagicResistance { get; set; }
        public byte Accuracy { get; set; }
        public byte Dodge { get; set; }
        public uint HPRegen { get; set; }
        public ushort EnergyRegen { get; set; }
        public byte ImproveHealings { get; set; }
        public byte ReduceEnergyCost { get; set; }
        public uint LifeStealRating { get; set; }

        public uint ActionID0 { get; set; }
        public uint ActionID1 { get; set; }
        public uint ActionID2 { get; set; }
        public uint ActionID3 { get; set; }
        public uint ActionID4 { get; set; }
        public uint ActionID5 { get; set; }

        public CharacterActionSequence[] ActionSequences { get; set; }
        public uint[][] GearIDs { get; set; }
        public ushort EvolutionType { get; set; }
    }
}
