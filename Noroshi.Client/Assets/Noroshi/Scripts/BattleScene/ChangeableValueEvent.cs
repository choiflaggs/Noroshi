namespace Noroshi.BattleScene
{
    public class ChangeableValueEvent
    {
        public ChangeableValueEvent(int difference, int current, int max, bool ignoreAboveUI = false)
        {
            Difference = difference;
            Current    = current;
            Max        = max;
            IgnoreAboveUI = ignoreAboveUI;
        }
        public readonly int Difference;
        public readonly int Current;
        public readonly int Max;
        public readonly bool IgnoreAboveUI;
    }
}
