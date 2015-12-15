namespace Noroshi.Core.Game.Enums
{
    /// <summary>
    /// バトル種別
    /// </summary>
    public enum BattleCategory
    {
        Stage = 1,
        BackStage = 2,
        Arena = 3,
        Training = 4,
        Expedition = 5,
        DefensiveWar = 6,
        Trials = 7,
        RaidBoss = 8,
    }

    public enum BattleAutoMode
    {
        Manual,
        Selectable,
        Auto,
    }

    public enum VictoryOrDefeat
    {
        Win = 1,
        Loss = 2,
        Draw = 3,
        TimeUp = 4,
    }

    public enum DamageMagicalAttribute
    {
        Fire = 1,
        Water,
        Thunder,
        Earth,
        Wind,
    }
    public enum ActionTargetAttribute
    {
        Fire = 1,
        Water,
        Thunder,
        Earth,
        Wind,
        Flying,
        Machine,
        Animal,
    }
}
