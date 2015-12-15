using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;

namespace Noroshi.BattleScene.Drop
{
    /// 敵キャラクター撃破時にドロップするものを扱うクラス。
    public class DropHandler
    {
        readonly List<List<List<DropItem>>> _items;
        readonly Dictionary<byte, DropItem> _itemMap = new Dictionary<byte, DropItem>();
        byte _currentTotalDropItemNum;
        readonly uint _money;
        uint _currentTotalDropMoney;
        readonly byte _dropMoneyCharacterNum;
        readonly Subject<DropItemEvent> _onCommandDropItemSubject = new Subject<DropItemEvent>();
        readonly Subject<DropMoneyEvent> _onCommandDropMoneySubject = new Subject<DropMoneyEvent>();
        readonly CompositeDisposable _disposables = new CompositeDisposable();

        public DropHandler(List<List<List<Core.WebApi.Response.Possession.PossessionObject>>> dropPossessionObjects, uint gold, byte dropGoldCharacterNum)
        {
            // ドロップアイテム初期化
            var dropItems = dropPossessionObjects ?? new List<List<List<Core.WebApi.Response.Possession.PossessionObject>>>{
                new List<List<Core.WebApi.Response.Possession.PossessionObject>>{
                    new List<Core.WebApi.Response.Possession.PossessionObject>()
                }
            };
            byte dropItemNo = 1;
            _items = dropItems.Select(wi => wi.Select(ci => ci.Select(i => {
                var dropItem = new DropItem(dropItemNo++, i.ID, (byte)i.Num);
                _itemMap.Add(dropItem.No, dropItem);
                return dropItem;
            }).ToList()).ToList()).ToList();
            foreach (var dropItem in GetDropItems())
            {
                dropItem.GetOnCommandObservable().Subscribe(die => _onCommandDropItemSubject.OnNext(die))
                .AddTo(_disposables);
            }
            // ドロップマネー初期化
            _money = gold;
            _dropMoneyCharacterNum = dropGoldCharacterNum;
        }
        /// ドロップアイテムに対する命令 Observable を取得。
        public IObservable<DropItemEvent> GetOnCommandDropItemObservable()
        {
            return _onCommandDropItemSubject.AsObservable();
        }
        /// ドロップマネーに対する命令 Observable を取得。
        public IObservable<DropMoneyEvent> GetOnCommandDropMoneyObservable()
        {
            return _onCommandDropMoneySubject.AsObservable();
        }

        /// ドロップ（予定）アイテムを取得。
        public IEnumerable<DropItem> GetDropItems()
        {
            return _items.SelectMany(wi => wi.SelectMany(ci => ci));
        }
        /// ドロップ（予定）アイテム ID を取得。
        public IEnumerable<uint> GetDropItemIDs() { return GetDropItems().Select(i => i.ItemID); }
        public DropItem GetDropItem(byte no)
        {
            return _itemMap[no];
        }

        DropItem[] _getDropItem(byte waveNo, byte characterNo)
        {
            if (_items.Count() == 0) return new DropItem[0];
            if (_items.Count() < waveNo) return new DropItem[0];
            return _items[waveNo - 1][characterNo - 1].ToArray();
        }

        /// ドロップマネー総額を取得。
        public uint GetDropMoney() { return _money; }

        /// ドロップ処理。
        public void Drop(ICharacterView characterView, byte waveNo, byte characterNo)
        {
            _dropItems(characterView, waveNo, characterNo);
            _dropMoney(characterView);
        }
        void _dropItems(ICharacterView characterView, byte waveNo, byte characterNo)
        {
            var dropItems = _getDropItem(waveNo, characterNo);
            foreach (var dropItem in dropItems)
            {
                dropItem.Drop(characterView);
            }
        }
        void _dropMoney(ICharacterView characterView)
        {
            if (_money == 0) return;
            var money = _money / _dropMoneyCharacterNum;
            _currentTotalDropMoney += money;
            // 辻褄合わせ
            if (_money - _currentTotalDropMoney < money) _currentTotalDropMoney = _money;
            var dropMoney = new DropMoney(money);
            dropMoney.GetOnCommandObservable().Subscribe(die => _onCommandDropMoneySubject.OnNext(die))
            .AddTo(_disposables);
            dropMoney.Drop(characterView, _currentTotalDropMoney);
        }

        public void PickUpDropItem(byte no)
        {
            GetDropItem(no).PickUp(() => ++_currentTotalDropItemNum);
        }
        /// 強制ピックアップ。
        public void ForcePickUpDropItems()
        {
            var droppingItems = GetDropItems().Where(i => i.IsDropping).ToList();
            foreach (var no in droppingItems.Select(di => di.No))
            {
                PickUpDropItem(no);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}