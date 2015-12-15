using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Noroshi.BattleScene;
using Noroshi.BattleScene.UI;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class CharacterAboveUIView : UIView, ICharacterAboveUIView
    {
        [SerializeField] GaugeView _hpGauge;
        [SerializeField] GaugeView _shieldGauge;
        [SerializeField] Image _hpGamgeEffectFrame;
        [SerializeField] float _displayTime = 1f;

        [SerializeField] Sprite _damageEffectGaugeFrameSprite;
        [SerializeField] Sprite _healEffectGaugeFrameSprite;
        [SerializeField] Sprite _damagePlayerHpGaugeSprite;
        [SerializeField] Sprite _damageEnemyHpGaugeSprite;
        [SerializeField] Sprite _playerHpGaugeSprite;
        [SerializeField] Sprite _enemyHpGaugeSprite;
        [SerializeField] Sprite _healHpGaugeFillAlertSprite;

        readonly float _effectDuration = 0.1f;
        readonly int _hpGaugeLoopNum = 2;
        readonly int _hpGaugeEffectFrameLoopNum = 4;
        readonly Color _transparentColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        readonly float _doColorDuration = 1.0f;

        Func<Vector2> _getPosition;
        IDisposable _disappearDisposable;
        IDisposable _hpGaugeDisposable;
        IDisposable _shieldGaugeDisposable;
        Vector2 _hpGaugeDefaultAnchorPosition = Vector2.zero;
        // ゲージ画像判別用
        Force _force = Force.Own;

        new void Awake()
        {
            base.Awake();
            SetActive(false);
        }

        void Start()
        {
            _hpGaugeDefaultAnchorPosition = _hpGauge.GetRectTransform().anchoredPosition;
            _hpGauge.SetActive(false);
            _shieldGauge.SetActive(false);
        }

        void Update()
        {
            if (_getPosition != null)
            {
                SetPosition(_getPosition());
            }
        }

        public void SetTarget(ICharacterView icharacterView, Force force, uint initialHp, uint maxHp)
        {
            var characterView = (CharacterView)icharacterView;
            _getPosition = characterView.GetTopPosition;
            _force = force;
            SetActive(true);
            _hpGauge.SetValue((float)initialHp / maxHp);
            _shieldGauge.SetValue(0.0f);
        }

        public void ChangeHPRatio(float ratio)
        {
            if (_hpGaugeDisposable != null)
            {
                _hpGaugeDisposable.Dispose();
                _hpGauge.ResetAnimationEndValue();
            }

            var isHeal = _hpGauge.Value <= ratio; // true = heal, false = damage
            if (isHeal)
            {
                _hpGamgeEffectFrame.sprite = _healEffectGaugeFrameSprite;
                _hpGauge.FillAlertImage.sprite = _healHpGaugeFillAlertSprite;

                _hpGauge.GetOnValueChangeObservalbe().Subscribe(hpRatio => 
                {
                    _hpGauge.SetFillAlertValue(hpRatio);
                    if (hpRatio == ratio) _hpGauge.FillAlertImage.sprite = (_force == Force.Own) ? _playerHpGaugeSprite : _enemyHpGaugeSprite;
                });
            }
            else
            {
                _hpGamgeEffectFrame.sprite = _damageEffectGaugeFrameSprite;
                _hpGauge.FillAlertImage.sprite = (_force == Force.Own) ? _playerHpGaugeSprite : _enemyHpGaugeSprite;
                _hpGauge.FillImage.sprite = (_force == Force.Own) ? _damagePlayerHpGaugeSprite : _damageEnemyHpGaugeSprite;

                _hpGauge.SetFillAlertValue(ratio);

                _hpGauge.GetRectTransform().DOKill();
                _hpGauge.GetRectTransform().anchoredPosition = _hpGaugeDefaultAnchorPosition;
                _hpGauge.GetRectTransform().DOAnchorPos(Vector2.zero, _effectDuration).SetLoops(_hpGaugeLoopNum, LoopType.Yoyo).SetEase(Ease.OutExpo);
            }
            
            _hpGamgeEffectFrame.DOKill();
            _hpGamgeEffectFrame.color = _transparentColor;
            _hpGamgeEffectFrame.DOColor(Color.white, _effectDuration).SetLoops(_hpGaugeEffectFrameLoopNum, LoopType.Yoyo);

            _hpGaugeDisposable = _hpGauge.SetAnimationValue(ratio, _displayTime, isHeal).Subscribe();
            // 一定期間後に消す。
            _hpGauge.SetActive(true);
            _setDisappearTimer();
        }
        public void ChangeShieldRatio(float ratio)
        {
            if (ratio <= 0.0f)
            {
                _shieldGauge.SetActive(false);
                _shieldGauge.ResetAnimationEndValue();
                return;
            }
            var isHeal = _shieldGauge.Value < ratio; // true = heal, false = damage

            _shieldGauge.FillAlertImage.enabled = isHeal;
            if (isHeal)
            {
                _shieldGauge.FillAlertImage.DOKill();
                _shieldGauge.FillAlertImage.color = Color.white;
                _shieldGauge.FillAlertImage.DOColor(_transparentColor, _doColorDuration);
                _shieldGauge.SetValue(ratio);
            }
            else
            {
                if (_shieldGaugeDisposable != null)
                {
                    _shieldGaugeDisposable.Dispose();
                    _shieldGauge.ResetAnimationEndValue();
                }
                _shieldGaugeDisposable = _shieldGauge.SetAnimationValue(ratio, _displayTime, isHeal).Subscribe();
                _shieldGauge.GetOnValueChangeObservalbe()
                    .Where(shiledRatio => shiledRatio <= 0.0f)
                    .Subscribe(_ => _shieldGauge.SetActive(false));
            }

            _shieldGauge.SetActive(true);
        }
        void _setDisappearTimer()
        {
            if (_disappearDisposable != null) _disappearDisposable.Dispose();
            _disappearDisposable = SceneContainer.GetTimeHandler().Timer(_displayTime).Subscribe(_ =>
            {
                _hpGauge.SetActive(false);
            });
        }

        void OnDestroy()
        {
            base.Dispose();
            if (_disappearDisposable != null) _disappearDisposable.Dispose();
        }

        public void Reset()
        {
            // 分身キャラが使用していた場合データが残ったままになってしまうので、ここでクリア
            _getPosition = null;

            _shieldGauge.SetValue(0.0f);
            SetActive(false);
        }
    }
}
