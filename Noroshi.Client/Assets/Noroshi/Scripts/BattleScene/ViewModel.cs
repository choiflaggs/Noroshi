using UniRx;

namespace Noroshi.BattleScene
{
    public abstract class ViewModel<T> where T : MonoBehaviours.IView
    {
        protected T _view;

        public virtual IObservable<T> LoadView()
        {
            return _loadView().Do(v => _view = v);
        }

        abstract protected IObservable<T> _loadView();

        public virtual void Dispose()
        {
            _view.Dispose();
        }
    }
}
