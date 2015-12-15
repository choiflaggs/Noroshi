namespace Noroshi.Core.Game.Battle
{
    public class BattleResult
    {
        public AfterBattlePlayerCharacter[] AfterBattlePlayerCharacters { get; set; }
        public byte CurrentWaveNo { get; set; }
        public Wave CurrentWave { get; set; }
        public ushort DefeatingNum { get; set; }
        public uint Damage { get; set; }

        public class Wave
        {
            public EnemyCharacterState[] EnemyCharacterStates { get; set; }
        }
        public class EnemyCharacterState
        {
            public uint InitialHP { get; set; }
            public uint RemainingHP { get; set; }
        }
    }
}
