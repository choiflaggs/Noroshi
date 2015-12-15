using UniRx;
using UnityEngine;
using DG.Tweening;
using Noroshi.Grid;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ResultCharacterMessageUIView : UIView
    {
        [SerializeField] UnityEngine.UI.Text _text;
        [SerializeField] RectTransform _topLine;
        [SerializeField] RectTransform _bottomLine;
        [SerializeField] float _activeCharacterPositionX = 0f;
        [SerializeField] float _stopCharacterPositionX = -4f;
        [SerializeField] float _lineMoveDuration = 0.5f;
        [SerializeField] float _characterViewSlideDuration = 0.3f;

        bool _clickable = false;
        Subject<bool> _onClickSubject;
        Vector2 _topLinePosition;
        Vector2 _bottomLinePosition;
        Vector2 _topLineHidePosition;
        Vector2 _bottomLineHidePosition;

        new void Awake()
        {
            base.Awake();
            _topLinePosition = _topLine.anchoredPosition;
            _bottomLinePosition = _bottomLine.anchoredPosition;
            _topLineHidePosition = _topLine.anchoredPosition + Vector2.up * _topLine.rect.height;
            _bottomLineHidePosition = _bottomLine.anchoredPosition + Vector2.down * _bottomLine.rect.height;
            SetActive(false);
        }
        
        public IObservable<bool> Activate(ICharacterView characterView, string message, bool win)
        {
            return _activateLines()
                .SelectMany(_ => _activateCharacter(characterView, win))
                .SelectMany(_ => _activateMessage(message))
                .SelectMany(_ => _waitClick());
        }
        IObservable<bool> _activateLines()
        {
            SetActive(true);
            var onComplete = new Subject<bool>();
            _topLine.anchoredPosition = _topLineHidePosition;
            _topLine.DOAnchorPos(_topLinePosition, _lineMoveDuration).OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            _bottomLine.anchoredPosition = _bottomLineHidePosition;
            _bottomLine.DOAnchorPos(_bottomLinePosition, _lineMoveDuration);
            
            return onComplete.AsObservable();
        }

        IObservable<ICharacterView> _activateCharacter(ICharacterView characterView, bool win)
        {
            if (characterView == null) return Observable.Return<ICharacterView>(null);
            characterView.SetUpForUI(CenterPositionForUIType.Result);
            characterView.SetHorizontalDirection(win ? Direction.Right : Direction.Left);
            characterView.SetActive(true);
            var sign = win ? 1 : -1;
            return _moveLocalX(characterView, _activeCharacterPositionX * sign, _stopCharacterPositionX * sign, win);
        }

        IObservable<bool> _activateMessage(string message)
        {
            _text.text = message;
            return Observable.Return<bool>(true);
        }

        IObservable<bool> _waitClick()
        {
            _onClickSubject = new Subject<bool>();
            _clickable = true;
            return _onClickSubject.AsObservable();
        }

        void _deactivate()
        {
            _clickable = false;
        }

        IObservable<ICharacterView> _moveLocalX(ICharacterView characterView, float startValue, float endValue, bool win)
        {
            // TODO
            ((CharacterView)characterView).SetParent(UnityEngine.Camera.main.transform);
            characterView.SetUpForUI(CenterPositionForUIType.Result);

            var onComplete = new Subject<ICharacterView>();
            var prevLocalPosition = ((CharacterView)characterView).GetTransform().localPosition;
            ((CharacterView)characterView).GetTransform().localPosition = new Vector3(startValue, prevLocalPosition.y, -UnityEngine.Camera.main.transform.position.z);
            ((CharacterView)characterView).GetTransform().DOLocalMoveX(endValue - (endValue + prevLocalPosition.x * (win ? 1 : -1)), _characterViewSlideDuration).OnComplete(() =>
            {
                onComplete.OnNext(characterView);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable();
        }
        
        public void OnClick()
        {
            if (!_clickable) return;
            _onClickSubject.OnNext(true);
            _onClickSubject.OnCompleted();
        }
    }
}
