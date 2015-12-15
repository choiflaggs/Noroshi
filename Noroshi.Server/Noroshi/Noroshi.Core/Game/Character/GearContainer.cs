using System.Collections.Generic;
using System.Linq;

namespace Noroshi.Core.Game.Character
{
    public class GearContainer
    {
        Dictionary<byte, Gear> _gears = new Dictionary<byte, Gear>();

        public uint HP { get; private set; }
        public float StrengthGrowth { get; private set; }
        public float IntellectGrowth { get; private set; }
        public float AgilityGrowth { get; private set; }
        public uint Strength { get; private set; }
        public uint Intellect { get; private set; }
        public uint Agility { get; private set; }
        public float MagicCrit { get; private set; }
        public float HPRegen { get; private set; }
        public float EnergyRegen { get; private set; }
        public float Dodge { get; private set; }
        public float ArmorPenetration { get; private set; }
        public float LifeStealRating { get; private set; }
        public float ImproveHealings { get; private set; }
        public float IgnoreMagicResistance { get; private set; }
        public float PhysicalAttack { get; private set; }
        public float MagicPower { get; private set; }
        public float Armor { get; private set; }
        public float MagicResistance { get; private set; }
        public float PhysicalCrit { get; private set; }
        public float Accuracy { get; private set; }

        public Gear Get(byte slot)
        {
            return _gears.ContainsKey(slot) ? _gears[slot] : null;
        }
        public void Add(byte slot, Gear gear)
        {
            _gears.Add(slot, gear);
            _calculate();
        }
        public void Clear()
        {
            _gears.Clear();
            _calculate();
        }
        void _calculate()
        {
            HP = (uint)_gears.Values.Sum(g => g.HP);
            StrengthGrowth = _gears.Values.Sum(g => g.StrengthGrowth);
            IntellectGrowth = _gears.Values.Sum(g => g.IntellectGrowth);
            AgilityGrowth = _gears.Values.Sum(g => g.AgilityGrowth);
            Strength = (uint)_gears.Values.Sum(g => g.Strength);
            Intellect = (uint)_gears.Values.Sum(g => g.Intellect);
            Agility = (uint)_gears.Values.Sum(g => g.Agility);
            MagicCrit = _gears.Values.Sum(g => g.MagicCrit);
            HPRegen = _gears.Values.Sum(g => g.HPRegen);
            EnergyRegen = _gears.Values.Sum(g => g.EnergyRegen);
            Dodge = _gears.Values.Sum(g => g.Dodge);
            ArmorPenetration = _gears.Values.Sum(g => g.ArmorPenetration);
            LifeStealRating = _gears.Values.Sum(g => g.LifeStealRating);
            ImproveHealings = _gears.Values.Sum(g => g.ImproveHealings);
            IgnoreMagicResistance = _gears.Values.Sum(g => g.IgnoreMagicResistance);
            PhysicalAttack = _gears.Values.Sum(g => g.PhysicalAttack);
            MagicPower = _gears.Values.Sum(g => g.MagicPower);
            Armor = _gears.Values.Sum(g => g.Armor);
            MagicResistance = _gears.Values.Sum(g => g.MagicResistance);
            Accuracy = _gears.Values.Sum(g => g.Accuracy);
        }
    }
}