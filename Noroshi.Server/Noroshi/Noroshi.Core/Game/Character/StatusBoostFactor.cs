namespace Noroshi.Core.Game.Character
{
    public class StatusBoostFactor : IStatusBoostFactor
    {
        public float Strength { get; set; }
        public float Intellect { get; set; }
        public float Agility { get; set; }
        public int PhysicalAttack { get; set; }
        public int MagicPower { get; set; }
        public int Armor { get; set; }
        public int MagicRegistance { get; set; }
        public sbyte Accuracy { get; set; }
        public sbyte Dodge { get; set; }
        public int LifeStealRating { get; set; }
        public float ActionFrequency { get; set; }
        public int MaxHp { get; set; }
    }
}
