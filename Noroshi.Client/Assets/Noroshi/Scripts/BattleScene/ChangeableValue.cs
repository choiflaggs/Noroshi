using UniRx;

namespace Noroshi.BattleScene
{
    public class ChangeableValue
    {
        Subject<ChangeableValueEvent> _onChange = new Subject<ChangeableValueEvent>();

        public int Max     { get; protected set; }
        public int Current { get; protected set; }

        public IObservable<ChangeableValueEvent> GetOnChangeObservable()
        {
            return _onChange.AsObservable();
        }

        protected void _changeCurrent(int difference)
        {
            Current += difference;
            if (Current > Max) Current = Max;
            if (Current < 0) Current = 0;
            if (difference != 0) _onChange.OnNext(new ChangeableValueEvent(difference, Current, Max));
        }
        protected void _changeCurrentWithIgnoreAboveUI(int difference)
        {
            Current += difference;
            if (Current > Max) Current = Max;
            if (Current < 0) Current = 0;
            if (difference != 0) _onChange.OnNext(new ChangeableValueEvent(difference, Current, Max, true));
        }

        protected void _changeMax(int max)
        {
            if (Max != max)
            {
                if (Current == Max) Current = max;
                Max = max;
                _onChange.OnNext(new ChangeableValueEvent(0, Current, Max, true));
            }
        }
    }
}