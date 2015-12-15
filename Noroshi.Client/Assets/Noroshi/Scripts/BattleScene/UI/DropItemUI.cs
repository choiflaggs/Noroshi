using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class DropItemUI : UIViewModel<IDropItemUIView>
    {
        public readonly uint ItemID;
        public readonly byte No;

        public DropItemUI(uint itemId, byte dropItemNo)
        {
            ItemID = itemId;
            No = dropItemNo;
        }

        public IObservable<DropItemUI> LoadView(IUIController uiController)
        {
            return LoadView().Do(v => uiController.SetToWorldUICanvas(v)).Select(_ => this);
        }
        protected override IObservable<IDropItemUIView> _loadView()
        {
            return SceneContainer.GetFactory().BuildDropItemUIView(ItemID).Do(v => v.SetActive(false));
        }

        public IObservable<DropItemUI> Drop(ICharacterView characterView)
        {
            _uiView.SetActive(true);
            return _uiView.Drop(characterView, No).Select(_ => this);
        }

        public IObservable<DropItemUI> Gain()
        {
            return _uiView.Gain().Select(v => {
                v.Dispose();
                _uiView = null;
                return this;
            });
        }
    }
}