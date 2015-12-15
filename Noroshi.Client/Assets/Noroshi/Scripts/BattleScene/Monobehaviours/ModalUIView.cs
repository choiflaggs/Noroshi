using UnityEngine;
using UniRx;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ModalUIView : UIView, UI.IModalUIView
    {
        enum ModalAnimationType
        {
            None = 0,
            Scale,
            Position
        }

        [SerializeField] protected UIView _window;
        [SerializeField] float _duration = 0.2f;
        [SerializeField] ModalAnimationType _modalAnimationType;
        [SerializeField] Vector2 _enterOpenAnimationValue = Vector2.zero;
        [SerializeField] Vector2 _exitOpenAnimationValue = Vector2.one;
        [SerializeField] Vector2 _exitCloseAnimationValue = Vector2.zero;

        Subject<bool> _clickCloseSubject = new Subject<bool>();

        /// クローズイベント用 Observable を取得するためのメソッド。
        public IObservable<bool> GetClickCloseObservable()
        {
            return _clickCloseSubject.AsObservable();
        }
		
        /// UnityEngine.UI.Button にセットするためのメソッド。クローズイベントをプッシュする。
        public void ClickClose()
        {
            _clickCloseSubject.OnNext(true);
        }

        /// モーダルを開くメソッド。演出は仮。
        public virtual IObservable<UI.IModalUIView> Open()
        {
            switch (_modalAnimationType)
            {
            case ModalAnimationType.None:
                return Observable.Return<UI.IModalUIView>(this);
            case ModalAnimationType.Scale:
                return _playWindowScaleAnimation(_enterOpenAnimationValue, _exitOpenAnimationValue);
            case ModalAnimationType.Position:
                return _playWindowPositionMoveAnimation(_enterOpenAnimationValue, _exitOpenAnimationValue);
            default:
                return _playWindowScaleAnimation(_enterOpenAnimationValue, _exitOpenAnimationValue);
            }
        }
		
        /// モーダルを閉じるメソッド。演出は仮。
        public IObservable<UI.IModalUIView> Close()
        {
            switch (_modalAnimationType)
            {
            case ModalAnimationType.None:
                return Observable.Return<UI.IModalUIView>(this);
            case ModalAnimationType.Scale:
                return _playWindowScaleAnimation(_exitOpenAnimationValue, _exitCloseAnimationValue);
            case ModalAnimationType.Position:
                return _playWindowPositionMoveAnimation(_exitOpenAnimationValue, _exitCloseAnimationValue);
            default:
                return _playWindowScaleAnimation(_exitOpenAnimationValue, _exitCloseAnimationValue);
            }
        }

        IObservable<UI.IModalUIView> _playWindowScaleAnimation(Vector2 inAnimValue, Vector2 finishAnimValue)
        {
            var subject = new Subject<UI.IModalUIView>();
            _window.transform.localScale = new Vector3(inAnimValue.x, inAnimValue.y, 1.0f);
            var endScale = new Vector3(finishAnimValue.x, finishAnimValue.y, 1.0f);
            _window.transform.DOScale(endScale, _duration).OnComplete(() => {
                subject.OnNext(this);
                subject.OnCompleted();
            });
            return subject.AsObservable<UI.IModalUIView>();
        }

        IObservable<UI.IModalUIView> _playWindowPositionMoveAnimation(Vector2 inAnimValue, Vector2 finishAnimValue)
        {
            var subject = new Subject<UI.IModalUIView>();
            _window.GetRectTransform().anchoredPosition = inAnimValue;
            _window.GetRectTransform().DOAnchorPos(finishAnimValue, _duration).OnComplete(() => {
                subject.OnNext(this);
                subject.OnCompleted();
            });
            return subject.AsObservable<UI.IModalUIView>();
        }
    }
}