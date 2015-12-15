namespace Noroshi.Core.Game.Battle
{
    public class Constant
    {
        public const int MAX_CHARACTER_NUM_PER_DECK = 5;

        /// <summary>
        /// 軍当たりの最大デカキャラ数。
        /// </summary>
        public const byte MAX_DECA_CHARACTER_NUM_PER_FORCE = 2;
        /// <summary>
        /// 次のウェーブに遷移する際に回復する HP 割合（最大 HP 比）
        /// </summary>
        public const float HP_RECOVERY_WAVE_INTERVAL_RATIO = 0.3f;
        /// <summary>
        /// 最大エネルギー。
        /// </summary>
        public const ushort MAX_ENERGY = 1000;
        /// <summary>
        /// 次のウェーブに遷移する際に回復するエネルギー割合（最大エネルギー比）
        /// </summary>
        public const float ENERGY_RECOVERY_WAVE_INTERVAL_RATIO = 0.3f;
    }
}
