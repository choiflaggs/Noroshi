namespace Noroshi.BattleScene
{
    public class CharacterEnergy : ChangeableValue
    {
        /// 最大エネルギー
        public const int MAX_ENERGY = 1000;
        /// ダメージを与えた際に回復するエネルギー係数
        public const float ENERGY_RECOVERY_EXECUTOR_DAMAGE_COEFFICIENT = 300f;
        /// 相手を死亡させた際に回復するエネルギー割合（最大エネルギー比）
        public const float ENERGY_RECOVERY_EXECUTOR_KILL_RATIO = 0.4f;
        /// ダメージを受けた際に回復するエネルギー係数
        public const float ENERGY_RECOVERY_TARGET_DAMAGE_COEFFICIENT = 1000f;
        /// 次のウェーブに遷移する際に回復するエネルギー割合（最大エネルギー比）
        public const float ENERGY_RECOVERY_WAVE_INTERVAL_RATIO = 0.3f;

        ushort _regen;

        public CharacterEnergy(ushort regen)
        {
            Max = MAX_ENERGY;
            Current = 0;
            _regen = regen;
        }

        public void ForceInitialize(uint initialValue)
        {
            Current = (int)initialValue;
            if (Current > Max) Current = Max;
        }

        public bool IsFull { get { return Current == Max; } }

        public void Damage(int damage)
        {
            _changeCurrent(-damage);
        }

        public void RecoverWhenSendDamage(int damage, uint maxHp, bool kill)
        {
            var energy = (float)damage / maxHp * ENERGY_RECOVERY_EXECUTOR_DAMAGE_COEFFICIENT;
            if (kill) energy += Max * ENERGY_RECOVERY_EXECUTOR_KILL_RATIO;
            _changeCurrentWithIgnoreAboveUI((int)energy);
        }
        public void RecoverWhenReceiveDamage(int damage, int maxHp)
        {
            _changeCurrentWithIgnoreAboveUI((int)((float)damage / maxHp * ENERGY_RECOVERY_TARGET_DAMAGE_COEFFICIENT));
        }
        public void RecoverWhenMoveNextWave()
        {
            _changeCurrent((int)(Max * ENERGY_RECOVERY_WAVE_INTERVAL_RATIO + _regen));
        }
        public void Consume(float reduceEnergyCost)
        {
            var consumingEnergy = (int)(Max * (1 - reduceEnergyCost));
            _changeCurrent(-consumingEnergy);
        }
        public void Reset()
        {
            _changeCurrent(-Current);
        }
        public void RecoverFully()
        {
            _changeCurrent(Max);
        }
    }
}
