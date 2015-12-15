using UniRx;

namespace Noroshi.BattleScene.UI
{
    public abstract class UIViewModel<T> where T : MonoBehaviours.IUIView
    {
        protected T _uiView;

        public virtual IObservable<T> LoadView()
        {
            return _loadView().Do(v => _uiView = v);
        }

        abstract protected IObservable<T> _loadView();

        public bool SetViewActive(bool active)
        {
            return _uiView.SetActive(active);
        }

        public virtual void Dispose()
        {
            _uiView.Dispose();
        }
    }
}
