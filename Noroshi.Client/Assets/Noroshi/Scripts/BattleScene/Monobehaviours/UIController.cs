using UniRx;
using UnityEngine;
using DG.Tweening;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class UIController : MonoBehaviour, IUIController
    {
        const float HEADER_AND_FOOTER_MOVE_DURATION = 0.5f;
        const float DARKEN_ALPHA = 0.7f;
        const float DARKEN_LIGHTEN_DURATION = 0.5f;

        [SerializeField] RectTransform _headerContainer;
        [SerializeField] RectTransform _footerContainer;
        [SerializeField] UIView _modalContainer;
        [SerializeField] UIView _bottomPanelContainer;
        [SerializeField] UnityEngine.UI.Text _gainMoneyNum;
        [SerializeField] UnityEngine.UI.Text _gainItemNum;
        [SerializeField] UnityEngine.UI.Text _currentWaveNum;
        [SerializeField] UnityEngine.UI.Text _maxWaveNum;
        [SerializeField] UIView _nextWaveButton;
        [SerializeField] LoadingUIView _loadingUIView;
        [SerializeField] TimeUIView _timeUIView;
        [SerializeField] SwapImageToggleView _autoModeToggleView;
        [SerializeField] UIView _worldUICanvas;
        [SerializeField] UIView _readyMessage;
        [SerializeField] WinMessageUIView _winMessageUIView;
        [SerializeField] ChapterUIView _chapterUIView;
        [SerializeField] DarkScreenUIView _darkScreenUIView;
        [SerializeField] ResultCharacterMessageUIView _resultCharacterMessageUIView;

        [SerializeField] UIView _headerWaveImageUIView;
        [SerializeField] WaveGaugeUIView _waveGaugeUIView;
        [SerializeField] UIView _textUICanvas;

        Vector2 _headerAnchoredPosition;
        Vector2 _footerAnchoredPosition;
        sbyte _darkScreenCallCount = 0;
        sbyte _deactiveHeaderAndFooterCallCount = 0;

        Subject<bool> _onNextWaveSubject = new Subject<bool>();
        Subject<bool> _onPauseSubject = new Subject<bool>();
        Subject<bool> _onToggleAutoModeSubject = new Subject<bool>();
        CompositeDisposable _disposables = new CompositeDisposable();

        new void Awake()
        {
            base.Awake();
            _loadingUIView.SetActive(true);
            SetCurrentMoneyNum(0);
            SetCurrentItemNum(0);
            _autoModeToggleView.SetActive(false);
            _darkScreenCallCount = 0;
            _deactiveHeaderAndFooterCallCount = 0;
        }
        void Start()
        {
            _headerAnchoredPosition = _headerContainer.anchoredPosition;
            _footerAnchoredPosition = _footerContainer.anchoredPosition;

            _winMessageUIView.SetActive(false);
            _readyMessage.SetActive(false);
        }

        public void DeactiveLoadingUIView()
        {
            _loadingUIView.SetActive(false);
        }

        public IObservable<bool> ActivateHeaderAndFooter()
        {
            if (--_deactiveHeaderAndFooterCallCount > 0) return Observable.Return<bool>(true);
            var onComplete = new Subject<bool>();
            _headerContainer.DOAnchorPos(_headerAnchoredPosition, HEADER_AND_FOOTER_MOVE_DURATION).OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            _footerContainer.DOAnchorPos(_footerAnchoredPosition, HEADER_AND_FOOTER_MOVE_DURATION);
            return onComplete.AsObservable();
        }
        public IObservable<bool> DeactivateHeaderAndFooter()
        {
            ++_deactiveHeaderAndFooterCallCount;
            var onComplete = new Subject<bool>();
            _headerContainer.DOAnchorPos(_headerAnchoredPosition + Vector2.up * _headerContainer.rect.height, HEADER_AND_FOOTER_MOVE_DURATION).OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            _footerContainer.DOAnchorPos(_footerAnchoredPosition + Vector2.down * _footerContainer.rect.height, HEADER_AND_FOOTER_MOVE_DURATION);
            return onComplete.AsObservable();
        }

        public void AddModalUIView(IModalUIView uiView)
        {
            uiView.SetParent(_modalContainer, false);
        }
        public void AddResultUIView(IResultUIView uiView)
        {
            uiView.SetParent(_modalContainer, false);
        }
        public void AddPlayerCharacterPanelUI(IOwnCharacterPanelUIView uiView)
        {
            uiView.SetParent(_bottomPanelContainer, false);
        }
        public void SetCurrentMoneyNum(uint num)
        {
            _gainMoneyNum.text = num.ToString();
        }
        public void SetCurrentItemNum(byte num)
        {
            _gainItemNum.text = num.ToString();
        }

        public void SetCurrentWaveNum(int num)
        {
            _currentWaveNum.text = num.ToString();
        }
        public void SetMaxWaveNum(int num)
        {
            _maxWaveNum.text = num.ToString();
        }

        public void SetNextWaveButtonVisible(bool visible)
        {
            _nextWaveButton.SetActive(visible);
        }

        public void InitializeWaveGaugeView(byte? level, string textKey, uint nowHP, uint maxHP, Noroshi.Core.Game.Battle.WaveGaugeType waveGaugeType)
        {
            _waveGaugeUIView.SetActive(true);
            _headerWaveImageUIView.SetActive(false);
            _waveGaugeUIView.Initialize(level.Value, textKey, (float)nowHP / maxHP, waveGaugeType);
        }

        public void ChangeWaveGauge(float ratio)
        {
            _waveGaugeUIView.ChangeHpRatio(ratio);
        }

        public IObservable<bool> GetOnClickNextWaveButtonObservable()
        {
            return _onNextWaveSubject.AsObservable();
        }
        public void ClickNextWaveButton()
        {
            _onNextWaveSubject.OnNext(true);
        }

        public IObservable<bool> GetOnClickPauseButtonObservable()
        {
            return _onPauseSubject.AsObservable();
        }
        public void ClickPauseButton()
        {
            _onPauseSubject.OnNext(true);
        }

        public IObservable<bool> GetOnToggleAutoModeObservable()
        {
            _autoModeToggleView.SetActive(true);
            return _autoModeToggleView.GetOnToggleObservable()
            .Do(isOn =>
            {
                if (isOn) _onNextWaveSubject.OnNext(true);
            });
        }

        public void UpdateTime(int time)
        {
            _timeUIView.UpdateTime(time);
        }

        public void SetToWorldUICanvas(MonoBehaviours.IUIView uiView)
        {
            uiView.SetParent(_worldUICanvas, false);
        }

        public IUIView GetTextUICanvas()
        {
            return _textUICanvas;
        }

        public IObservable<float> PlayPrepareAnimation()
        {
            _readyMessage.SetActive(true);
            var time = Time.time;
            return SceneContainer.GetTimeHandler().Timer(Constant.BATTLE_READY_ANIMATION_TIME)
            .Do(_ => _readyMessage.SetActive(false))
            .Select(_ => Time.time - time);
        }

        public void PlayWinMessage()
        {
            _winMessageUIView.SetActive(true);
            _winMessageUIView.PlayWinMessageAnimation()
                .Do(_ => _winMessageUIView.SetActive(false))
                .Subscribe().AddTo(_disposables);
        }

        public IObservable<IUIController> ActivateChapterUIView(string titleTextKey)
        {
            return _chapterUIView.Activate(titleTextKey).Select(_ => (IUIController)this);
        }

        public IObservable<IUIController> ActivateResultCharacterMessageUIView(ICharacterView characterView, string message, bool win)
        {
            return _resultCharacterMessageUIView.Activate(characterView, message, win).Select(_ => (IUIController)this);
        }

        public IObservable<bool> DarkenWorld()
        {
            ++_darkScreenCallCount;
            return _darkScreenUIView.Activate(DARKEN_ALPHA, DARKEN_LIGHTEN_DURATION).Select(_ => true);
        }

        public IObservable<bool> LightenWorld()
        {
            if (--_darkScreenCallCount > 0) return Observable.Return<bool>(true);
            return _darkScreenUIView.Deactivate(DARKEN_LIGHTEN_DURATION).Select(_ => true);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}