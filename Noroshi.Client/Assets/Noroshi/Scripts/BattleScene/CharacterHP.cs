namespace Noroshi.BattleScene
{
    public class CharacterHP : ChangeableValue
    {
        /// 次のウェーブに遷移する際に回復するHP割合（最大エネルギー比）
        public const float HP_RECOVERY_WAVE_INTERVAL_RATIO = 0.3f;

        uint _regen;

        public uint InitialHP { get; private set; }

        public CharacterHP(uint maxHp, uint regen)
        {
            Max = (int)maxHp;
            Current = Max;
            _regen = regen;
            InitialHP = (uint)Max;
        }

        public void ForceInitialize(uint initialValue)
        {
            Current = (int)initialValue;
            if (Current > Max) Current = Max;
            InitialHP = initialValue;
        }

        public void ChangeMaxHP(uint maxHp)
        {
            _changeMax((int)maxHp);
        }

        public void Damage(int damage)
        {
            _changeCurrent(-damage);
        }
        public void RecoverWhenLifeSteal(uint hp)
        {
            _changeCurrent((int)hp);
        }
        public void RecoverWhenMoveNextWave()
        {
            _changeCurrent((int)(Max * (HP_RECOVERY_WAVE_INTERVAL_RATIO + _regen)));
        }
        public void Reset()
        {
            Current = Max;
        }
    }
}