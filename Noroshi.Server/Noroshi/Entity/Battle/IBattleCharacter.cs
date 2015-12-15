namespace Noroshi.Server.Entity.Battle
{
    public interface IBattleCharacter
    {
        uint CharacterID { get; }
        ushort Level { get; }
        byte PromotionLevel { get; }
        byte EvolutionLevel { get; }
        ushort ActionLevel1 { get; }
        ushort ActionLevel2 { get; }
        ushort ActionLevel3 { get; }
        ushort ActionLevel4 { get; }
        ushort ActionLevel5 { get; }
        uint GearID1 { get; }
        uint GearID2 { get; }
        uint GearID3 { get; }
        uint GearID4 { get; }
        uint GearID5 { get; }
        uint GearID6 { get; }
    }
}
