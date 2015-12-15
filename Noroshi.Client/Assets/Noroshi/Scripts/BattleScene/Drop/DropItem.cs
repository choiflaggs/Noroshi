using UniRx;

namespace Noroshi.BattleScene.Drop
{
    public class DropItem
    {
        readonly Subject<DropItemEvent> _onCommandSubject = new Subject<DropItemEvent>();
        public readonly byte No;
        public readonly uint ItemID;
        public readonly byte ItemNum;
        public bool IsDropping { get; private set; }

        public DropItem(byte dropItemNo, uint itemId, byte itemNum)
        {
            No = dropItemNo;
            ItemID = itemId;
            ItemNum = itemNum;
        }

        public IObservable<DropItemEvent> GetOnCommandObservable()
        {
            return _onCommandSubject.AsObservable();
        }

        public void Drop(ICharacterView characterView)
        {
            IsDropping = true;
            _onCommandSubject.OnNext(new DropItemEvent(){
                Command = DropCommand.Drop,
                No = No,
                ItemID = ItemID,
                ItemNum = ItemNum,
                CharacterView = characterView,
            });
        }

        public void PickUp(System.Func<byte> currentTotalItemNumFunc)
        {
            IsDropping = false;
            _onCommandSubject.OnNext(new DropItemEvent(){
                Command = DropCommand.PickUp,
                No = No,
                ItemID = ItemID,
                ItemNum = ItemNum,
                CurrentTotalNumFunc = currentTotalItemNumFunc,
            });
        }
    }
}