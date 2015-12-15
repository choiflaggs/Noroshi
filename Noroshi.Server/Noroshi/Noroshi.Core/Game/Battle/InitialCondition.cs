namespace Noroshi.Core.Game.Battle
{
    public class InitialCondition
    {
        public PlayerCharacterCondition[] OwnPlayerCharacterConditions { get; set; }
        public CharacterCondition[][] EnemyCharacterConditions { get; set; }

        public class PlayerCharacterCondition
        {
            public uint PlayerCharacterID { get; set; }
            public uint? HP { get; set; }
            public ushort? Energy { get; set; }
            public float? DamageCoefficient { get; set; }
        }
        public class CharacterCondition
        {
            public uint? HP { get; set; }
            public ushort? Energy { get; set; }
        }
    }
}
