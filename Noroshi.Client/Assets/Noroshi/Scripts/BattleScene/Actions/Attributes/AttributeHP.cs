namespace Noroshi.BattleScene
{
    public class AttributeHP : ChangeableValue
    {
        public AttributeHP(int maxHp)
        {
            Max = maxHp;
            Current = Max;
        }

        public void Damage(int damage)
        {
            _changeCurrent(-damage);
        }
        public void Reset()
        {
            Current = 0;
            _changeCurrent(Max);
        }

        public void Clean()
        {
            if (Current > 0) Damage(Current);
        }
    }
}
