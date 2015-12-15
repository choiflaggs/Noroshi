namespace Noroshi.Core.Game.Character
{
    public interface IStatusBoostFactor
    {
        float Strength { get; }
        float Intellect { get; }
        float Agility { get; }
        int PhysicalAttack { get; }
        int MagicPower { get; }
        int Armor { get; }
        int MagicRegistance { get; }
        sbyte Accuracy { get; }
        sbyte Dodge { get; }
        int LifeStealRating { get; }
        float ActionFrequency { get; }
        int MaxHp { get; }
    }
}
