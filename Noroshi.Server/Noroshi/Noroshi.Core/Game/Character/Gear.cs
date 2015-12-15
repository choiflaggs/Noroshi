namespace Noroshi.Core.Game.Character
{
    public class Gear
    {
        readonly WebApi.Response.Gear _data;
        public Gear(WebApi.Response.Gear data)
        {
            _data = data;
        }

        public uint ID
        { get { return _data.ID; } }

        public ushort Level { get { return _data.Level; }}

        public uint HP { get { return _data.HP; } }

        public float StrengthGrowth
        { get { return _data.StrengthGrowth; } }
        public float IntellectGrowth
        { get { return _data.IntellectGrowth; } }
        public float AgilityGrowth
        { get { return _data.AgilityGrowth; } }
        public float Strength
        { get { return _data.Strength; } }
        public float Intellect
        { get { return _data.Intellect; } }
        public float Agility
        { get { return _data.Agility; } }
        public uint MagicCrit
        { get { return _data.MagicCrit; } }
        public float HPRegen
        { get { return _data.HPRegen; } }
        public float EnergyRegen
        { get { return _data.EnergyRegen; } }
        public float Dodge
        { get { return _data.Dodge; } }
        public uint ArmorPenetration
        { get { return _data.ArmorPenetration; } }
        public float LifeStealRating
        { get { return _data.LifeStealRating; } }
        public float ImproveHealings
        { get { return _data.ImproveHealings;  } }
        public uint IgnoreMagicResistance
        { get { return _data.IgnoreMagicResistance; } }
        public uint PhysicalAttack
        { get { return _data.PhysicalAttack; } }
        public uint MagicPower
        { get { return _data.MagicPower; } }
        public uint Armor
        { get { return _data.Armor; } }
        public uint MagicResistance
        { get { return _data.MagicResistance; } }
        public uint PhysicalCrit
        { get { return _data.PhysicalCrit; } }
        public byte Accuracy
        { get { return _data.Accuracy; } }
    }
}