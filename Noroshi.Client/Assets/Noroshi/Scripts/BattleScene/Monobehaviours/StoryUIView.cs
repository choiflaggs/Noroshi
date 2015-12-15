using UniRx;
using UnityEngine;
using DG.Tweening;
using Noroshi.Grid;
using System;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class StoryUIView : UIView, UI.IStoryUIView
    {
        [SerializeField] UnityEngine.UI.Text _text;
        [SerializeField] RectTransform _topLine;
        [SerializeField] RectTransform _bottomLine;
        [SerializeField] float _activeCharacterPositionX = -2.5f;
        [SerializeField] float _inactiveCharacterPositionX = -7.5f;
        [SerializeField] float _lineMoveDuration = 1f;
        [SerializeField] float _characterViewSlideDuration = 0.3f;
        [SerializeField] UnityEngine.UI.Text _characterName;
        [SerializeField] UnityEngine.UI.Toggle _autoModeToggle;
        // 自動テキスト送り時の間隔
        [SerializeField] float _autoModeIntervalTime = 3.5f;

        bool _clickable = false;
        Subject<bool> _onClickSubject;
        Vector2 _topLinePosition;
        Vector2 _bottomLinePosition;
        Vector2 _topLineHidePosition;
        Vector2 _bottomLineHidePosition;

        ICharacterView _currentOwnCharacterView;
        ICharacterView _currentEnemyCharacterView;

        IDisposable _autoModeIntervalDisposable = null;
        Subject<bool> _onIntervalTimeSubject = new Subject<bool>();

        new void Awake()
        {
            base.Awake();
            _topLinePosition = _topLine.anchoredPosition;
            _bottomLinePosition = _bottomLine.anchoredPosition;
            _topLineHidePosition = _topLine.anchoredPosition + Vector2.up * _topLine.rect.height;
            _bottomLineHidePosition = _bottomLine.anchoredPosition + Vector2.down * _bottomLine.rect.height;
            _autoModeToggle.onValueChanged.AddListener(_onAutoClick);
            SetActive(false);
        }

        public IObservable<bool> Begin()
        {
            SetActive(true);
            _topLine.anchoredPosition = _topLineHidePosition;
            _topLine.DOAnchorPos(_topLinePosition, _lineMoveDuration).OnComplete(() =>
            {
                _clickable = true;
                OnScreenClick();
            });

            _bottomLine.anchoredPosition = _bottomLineHidePosition;
            _bottomLine.DOAnchorPos(_bottomLinePosition, _lineMoveDuration);

            var onClickSubject = new Subject<bool>();
            _onClickSubject = onClickSubject;

            return onClickSubject.AsObservable();
        }

        public IObservable<UI.IStoryUIView> GoNext(ICharacterView ownCharacterView, ICharacterView enemyCharacterView, string textKey, string characterNameTextKey)
        {
            _startAutoMode();
            if (_autoModeToggle.isOn) _onIntervalTimeSubject.OnNext(true);
            return Observable.WhenAll(
                _updateOwnCharacter(_currentOwnCharacterView, ownCharacterView),
                _updateEnemyCharacter(_currentEnemyCharacterView, enemyCharacterView)
            )
            .Do(_ => 
            {
                _characterName.text = GlobalContainer.LocalizationManager.GetText(characterNameTextKey + ".Name");
                _text.text = GlobalContainer.LocalizationManager.GetText(textKey);
            })
            .Select(_ => (UI.IStoryUIView)this);
        }
        IObservable<ICharacterView> _updateOwnCharacter(ICharacterView previous, ICharacterView current)
        {
            if (_currentOwnCharacterView == current)
            {
                return Observable.Empty<ICharacterView>();
            }
            _currentOwnCharacterView = current;
            return _deactivateOwnCharacter(previous).SelectMany(_activateOwnCharacter(current));
        }
        IObservable<ICharacterView> _updateEnemyCharacter(ICharacterView previous, ICharacterView current)
        {
            if (_currentEnemyCharacterView == current)
            {
                return Observable.Empty<ICharacterView>();
            }
            _currentEnemyCharacterView = current;
            return _deactivateEnemyCharacter(previous).SelectMany(_activateEnemyCharacter(current));
        }

        public IObservable<bool> Finish()
        {
            return Observable.WhenAll(
                _deactivateOwnCharacter(_currentOwnCharacterView),
                _deactivateEnemyCharacter(_currentEnemyCharacterView)
            )
            .SelectMany(_finish());
        }
        IObservable<bool> _finish()
        {
            _clickable = false;
            var onFinish = new Subject<bool>();
            _topLine.DOAnchorPos(_topLineHidePosition, _lineMoveDuration).OnComplete(() =>
            {
                onFinish.OnNext(true);
                onFinish.OnCompleted();
                SetActive(false);
            });
            _bottomLine.DOAnchorPos(_bottomLineHidePosition, _lineMoveDuration);
            return onFinish.AsObservable().Do(_ => _onFinish());
        }
        void _onFinish()
        {
            _currentOwnCharacterView = null;
            _currentEnemyCharacterView = null;
            _characterName.text = string.Empty;
            _text.text = "";
            _clear();
        }

        IObservable<ICharacterView> _activateOwnCharacter(ICharacterView characterView)
        {
            if (characterView == null) return Observable.Empty<ICharacterView>();
            characterView.SetHorizontalDirection(Direction.Right);
            characterView.SetActive(true);
            return _moveLocalOwnX(characterView, _inactiveCharacterPositionX, _activeCharacterPositionX, true);
        }
        IObservable<ICharacterView> _deactivateOwnCharacter(ICharacterView characterView)
        {
            if (characterView == null) return Observable.Empty<ICharacterView>();
            return _moveLocalOwnX(characterView, _activeCharacterPositionX, _inactiveCharacterPositionX, false).Do(v => v.SetActive(false));
        }
        IObservable<ICharacterView> _activateEnemyCharacter(ICharacterView characterView)
        {
            if (characterView == null) return Observable.Empty<ICharacterView>();
            characterView.SetHorizontalDirection(Direction.Left);
            characterView.SetActive(true);
            return _moveLocalEnemyX(characterView, -_inactiveCharacterPositionX, -_activeCharacterPositionX, true);
        }
        IObservable<ICharacterView> _deactivateEnemyCharacter(ICharacterView characterView)
        {
            if (characterView == null) return Observable.Empty<ICharacterView>();
            return _moveLocalEnemyX(characterView, -_activeCharacterPositionX, -_inactiveCharacterPositionX, false).Do(v => v.SetActive(false));
        }

        IObservable<ICharacterView> _moveLocalOwnX(ICharacterView characterView, float startValue, float endValue, bool isInMove)
        {
            var onComplete = new Subject<ICharacterView>();
            characterView.SetUpForUI(CenterPositionForUIType.Story);
            var prevLocalPosition = ((CharacterView)characterView).GetTransform().localPosition;
            ((CharacterView)characterView).GetTransform().localPosition = new Vector3(startValue - ((startValue * (isInMove ? -1 : 1) + prevLocalPosition.x)), prevLocalPosition.y, prevLocalPosition.z);
            ((CharacterView)characterView).GetTransform().DOLocalMoveX(endValue - ((endValue * (isInMove ? 1 : -1) + prevLocalPosition.x)), _characterViewSlideDuration).OnComplete(() =>
            {
                onComplete.OnNext(characterView);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable();
        }

        IObservable<ICharacterView> _moveLocalEnemyX(ICharacterView characterView, float startValue, float endValue, bool isInMove)
        {
            var onComplete = new Subject<ICharacterView>();
            characterView.SetUpForUI(CenterPositionForUIType.Story);
            var prevLocalPosition = ((CharacterView)characterView).GetTransform().localPosition;
            var transform = ((CharacterView)characterView).GetTransform();
            transform.localPosition = new Vector3(startValue + ((startValue * (isInMove ? 1 : -1) + prevLocalPosition.x)), prevLocalPosition.y, prevLocalPosition.z);
            transform.DOLocalMoveX(endValue + ((endValue * (isInMove ? -1 : 1) + prevLocalPosition.x)), _characterViewSlideDuration).OnComplete(() =>
            {
                onComplete.OnNext(characterView);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable();
        }

        void _clear()
        {
            if (_autoModeIntervalDisposable != null)
            {
                _autoModeIntervalDisposable.Dispose();
                _autoModeIntervalDisposable = null;
            }
        }

        // 画面タップ時に呼ばれる
        public void OnScreenClick()
        {
            if (!_clickable) return;
            _onClickSubject.OnNext(true);
        }

        void _startAutoMode()
        {
            _clear();
            _autoModeIntervalDisposable = SceneContainer.GetTimeHandler().Timer(_autoModeIntervalTime)
            .Zip(_onIntervalTimeSubject.AsObservable(), (t1, t2) => 
            {
                return true;
            })
            .Subscribe(_ => OnScreenClick())
            .AddTo(this);
        }

        // 自動テキスト送りボタン(Toggle)を押した時に呼ばれる
        void _onAutoClick(bool isAutoMode)
        {
            if (isAutoMode)
            {
                _onIntervalTimeSubject.OnNext(true);
            }
            else
            {
                _startAutoMode();
            }
        }
    }
}