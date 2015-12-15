using UniRx;

namespace Noroshi.BattleScene.UI
{
    /// モーダル用抽象クラス。
    public abstract class AbstractModalUIViewModel<T> : UIViewModel<T> where T : IModalUIView
    {
        Subject<bool> _onOpenExitSubject = new Subject<bool>();
        Subject<bool> _onCloseEnterSubject = new Subject<bool>();
        Subject<bool> _onCloseExitSubject = new Subject<bool>();
        CompositeDisposable _compositeDisposable = new CompositeDisposable();

        /// ビューをロードするメソッド。ロードと同時にビューからのクローズイベントを登録。
        public override IObservable<T> LoadView()
        {
            return base.LoadView().Do(v =>
            {
                v.GetClickCloseObservable().Subscribe(_onClickClose).AddTo(_compositeDisposable);
            });
        }

        void _onClickClose(bool click)
        {
            Close().Subscribe();
        }

        /// モーダルが開いたタイミングでプッシュされる Observable を取得するメソッド。
        public IObservable<bool> GetOnOpenExitObservable()
        {
            return _onOpenExitSubject.AsObservable();
        }

        /// モーダルを閉じ始めたタイミングでプッシュされる Observable を取得するメソッド。
        public IObservable<bool> GetOnCloseEnterObservable()
        {
            return _onCloseEnterSubject.AsObservable();
        }

        /// モーダルを閉じ切ったタイミングでプッシュされる Observable を取得するメソッド。
        public IObservable<bool> GetOnCloseExitObservable()
        {
            return _onCloseExitSubject.AsObservable();
        }

        /// モーダルを開くメソッド。
        public IObservable<AbstractModalUIViewModel<T>> Open()
        {
            SetViewActive(true);
            return _uiView.Open().Do(_ => _onOpenExit()).Select(_ => this);
        }

        /// モーダルを閉じるメソッド。
        public IObservable<AbstractModalUIViewModel<T>> Close()
        {
            _onCloseEnter();
            return _uiView.Close().Do(_ =>
            {
                _onCloseExit();
                SetViewActive(false);
            }).Select(_ => this);
        }

        void _onOpenExit()
        {
            _onOpenExitSubject.OnNext(true);
        }
        void _onCloseEnter()
        {
            _onCloseEnterSubject.OnNext(true);
        }
        void _onCloseExit()
        {
            _onCloseExitSubject.OnNext(true);
        }
    }
}