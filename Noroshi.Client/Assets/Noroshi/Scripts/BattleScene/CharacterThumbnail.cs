namespace Noroshi.BattleScene
{
    public class CharacterThumbnail
    {
        public CharacterThumbnail(uint characterId, ushort level, byte evolutionLevel, byte promotionLevel, byte skinLevel, bool isDead = false)
        {
            CharacterID = characterId;
            Level = level;
            EvolutionLevel = evolutionLevel;
            PromotionLevel = promotionLevel;
            IsDead = isDead;
            SkinLevel = skinLevel;
        }
        public readonly uint CharacterID;
        public readonly ushort Level;
        public readonly byte EvolutionLevel;
        public readonly byte PromotionLevel;
        public readonly bool IsDead;
        public readonly byte SkinLevel;
    }
}