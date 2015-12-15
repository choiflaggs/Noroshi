using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class DropMoneyUI : UIViewModel<IDropMoneyUIView>
    {
        ICharacterView _characterView;
        public int Value { get; private set; }

        public DropMoneyUI(int value, ICharacterView characterView)
        {
            Value = value;
            _characterView = characterView;
        }

        public IObservable<DropMoneyUI> LoadView(IUIController uiController)
        {
            return LoadView().Do(v => uiController.SetToWorldUICanvas(v)).Select(_ => this);
        }
        protected override IObservable<IDropMoneyUIView> _loadView()
        {
            return SceneContainer.GetFactory().BuildDropMoneyUIView();
        }

        public IObservable<DropMoneyUI> Drop()
        {
            return _uiView.Drop(_characterView).Select(_ => this);
        }
    }
}