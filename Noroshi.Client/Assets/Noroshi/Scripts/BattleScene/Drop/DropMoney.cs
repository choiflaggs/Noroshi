using UniRx;

namespace Noroshi.BattleScene.Drop
{
    public class DropMoney
    {
        readonly uint _money;
        readonly Subject<DropMoneyEvent> _onCommandSubject = new Subject<DropMoneyEvent>();

        public DropMoney(uint money)
        {
            _money = money;
        }

        public IObservable<DropMoneyEvent> GetOnCommandObservable()
        {
            return _onCommandSubject.AsObservable();
        }

        public void Drop(ICharacterView characterView, uint currentTotalMoney)
        {
            _onCommandSubject.OnNext(new DropMoneyEvent(){
                CurrentTotalMoney = currentTotalMoney,
                Money = _money,
                CharacterView = characterView,
            });
        }
    }
}