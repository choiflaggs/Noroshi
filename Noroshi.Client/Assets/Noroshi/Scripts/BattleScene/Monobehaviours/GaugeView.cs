using UnityEngine;
using UnityEngine.UI;
using Noroshi.BattleScene.UI;
using UniRx;
using UniRx.Triggers;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class GaugeView : UIView
    {
        [SerializeField] Image _fillImage;
        [SerializeField] Image _fillAlertImage;

        Subject<float> _onValueChangeSubject = new Subject<float>();
        Subject<float> _sliderAnimationSubject= new Subject<float>();
        Subject<bool> _onSliderAnimation = new Subject<bool>();
        System.IDisposable _sliderAnimationDisposable = null;
        Vector2 _defaultAnchorPos = Vector2.zero;
        float _value = 1.0f;
        float _defaultImageWidth = 0.0f;
        float _positionXBuffer = 3.5f;
        RectTransform _fillRectTransform = null;
        RectTransform _fillAlertRectTransform = null;
        float _animationEndValue = 0.0f;
        float _progressionSpeed = 1.0f;

        public float Value { get { return _value; } }
        public Image FillImage{ get{ return _fillImage; } }
        public Image FillAlertImage{ get { return _fillAlertImage; } }

        new void Awake()
        {
            base.Awake();
            _fillRectTransform = _fillImage.rectTransform;
            _fillAlertRectTransform = _fillAlertImage.rectTransform;
            _defaultImageWidth = _fillImage.sprite.rect.width;
            _defaultAnchorPos = _fillRectTransform.anchoredPosition;
            _rectTransform = GetComponent<RectTransform>();
        }

        void OnDestroy()
        {
            _onValueChangeSubject.OnCompleted();
            _sliderAnimationSubject.OnCompleted();
            _onSliderAnimation.OnCompleted();
            if (_sliderAnimationDisposable != null) _sliderAnimationDisposable.Dispose();
        }

        Vector2 _getNextAnchorPosition(float value)
        {
            if (_defaultImageWidth <= 0.0f) return _defaultAnchorPos;
            var x = (_defaultImageWidth * value) - _defaultImageWidth + _positionXBuffer;
            if (x > 0.0f) x = 0.0f;
            return new Vector2(x, _defaultAnchorPos.y);
        }

        void _setFillAnchor(Vector2 anchorPosition)
        {
            _fillRectTransform.anchoredPosition = anchorPosition;
            _onValueChangeSubject.OnNext(_value);
        }

        void _setFillAlertAnchor(Vector2 anchorPosition)
        {
            _fillAlertRectTransform.anchoredPosition = anchorPosition;
        }

        float GetRangeValue(float value)
        {
            var rangeValue = value;
            if (rangeValue > 1.0f) rangeValue = 1.0f;
            if (rangeValue < 0.0f) rangeValue = 0.0f;
            return rangeValue;
        }

        // 通常のゲージ画像と、その上に被せるゲージ画像を同じタイミングで動かす場合に使用する
        public void SetValue(float value)
        {
            _value = GetRangeValue(value);
            var anchorPosition = _getNextAnchorPosition(_value);
            _setFillAnchor(anchorPosition);
            _setFillAlertAnchor(anchorPosition);
        }

        // 通常のゲージだけ動かしたい時に使用する
        public void SetFillValue(float value)
        {
            _value = GetRangeValue(value);
            var anchorPosition = _getNextAnchorPosition(_value);
            _setFillAnchor(anchorPosition);
        }

        // 通常ゲージ画像の上に被せるゲージ画像のみを動かす場合に使用する
        public void SetFillAlertValue(float value)
        {
            // _valueは更新させない
            var fillAlertValue = GetRangeValue(value);
            var anchorPosition = _getNextAnchorPosition(fillAlertValue);
            _setFillAlertAnchor(anchorPosition);
        }

        public void ResetAnimationEndValue()
        {
            _animationEndValue = 0.0f;
            if (_sliderAnimationDisposable != null) _sliderAnimationDisposable.Dispose();
        }

        public IObservable<bool> SetAnimationValue(float endValue, float progressionSpeed, bool isAddtion)
        {
            _animationEndValue = endValue;
            _progressionSpeed = progressionSpeed;

            var valueChangeFinishObservable = (isAddtion) ? _getAdditionValueObservable() : _getSubtractionValueObservable();
            
            _sliderAnimationDisposable = _sliderAnimationSubject.ObserveEveryValueChanged(_ => _value)
                .TakeUntil(valueChangeFinishObservable)
                    .Subscribe(_ => {});
            if (_animationEndValue > 0.0f)
            {
                _sliderAnimationSubject.OnNext(0.0f);
            }
            else
            {
                _sliderAnimationSubject.OnCompleted();
                _onSliderAnimation.OnCompleted();
            }
            return _onSliderAnimation.AsObservable();
        }

        IObservable<Unit> _getSubtractionValueObservable()
        {
            return this.UpdateAsObservable()
                .SkipWhile(_ => float.IsNaN(_animationEndValue) || _animationEndValue < 0.0f)
                .Do(_ => 
                    {
                    var nowValue = _value - (Time.deltaTime * _progressionSpeed);
                    SetFillValue(nowValue);
                    var isValueOver = _value <= 0.0f;
                    if (isValueOver)
                    {
                        _animationEndValue -= 1.0f; 
                        SetValue(0.0f);
                        
                        if (_animationEndValue >= 0.0f)
                        {
                            _onSliderAnimation.OnNext(isValueOver);
                            if (_animationEndValue <= 0.0f) _onSliderAnimation.OnCompleted();
                        }
                        else
                        {
                            _onSliderAnimation.OnCompleted();
                        }
                    }
                    else
                    {
                        if (Value <= _animationEndValue)
                        {
                            SetFillValue(_animationEndValue);
                            _onSliderAnimation.OnCompleted();
                        }
                    }
                })
                .Where(_ => _value <= _animationEndValue);
        }

        IObservable<Unit> _getAdditionValueObservable()
        {
            return this.UpdateAsObservable()
                .SkipWhile(_ => float.IsNaN(_animationEndValue) || _animationEndValue <= 0.0f)
                .Do(_ => 
                {
                    var nowValue = _value + (Time.deltaTime * _progressionSpeed);
                    SetFillValue(nowValue);
                    var isValueOver = _value >= 1.0f;
                    if (isValueOver)
                    {
                        _animationEndValue -= 1.0f;
                        SetValue(1.0f);
                        
                        if (_animationEndValue >= 0.0f)
                        {
                            _onSliderAnimation.OnNext(isValueOver);
                            if (_animationEndValue <= 0.0f) _onSliderAnimation.OnCompleted();
                        }
                        else
                        {
                            _onSliderAnimation.OnCompleted();
                        }
                    }
                    else
                    {
                        if (Value >= _animationEndValue)
                        {
                            SetFillValue(_animationEndValue);
                            _onSliderAnimation.OnCompleted();
                        }
                    }
                })
                .Where(_ => _value >= _animationEndValue);
        }

        public IObservable<float> GetOnValueChangeObservalbe()
        {
            return _onValueChangeSubject.AsObservable();
        }
    }
}