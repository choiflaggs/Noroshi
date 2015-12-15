using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UniLinq;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class CharacterTextUIView : UIView, UI.ICharacterTextUIView
    {
        [SerializeField] float _textAppearY = 64.0f;
        [SerializeField] float _scaleAppearTime = 0.15f;
        [SerializeField] float _moveAppearTime = 0.5f;
        [SerializeField] float _moveDisappearTime = 1.0f;

        [SerializeField] Text _textUI;

        [SerializeField] Font _defaultFont;
        [SerializeField] Font _hpDamageFont;
        [SerializeField] Font _hpRecoveryFont;
        [SerializeField] Font _energyRecoveryFont;

        readonly int _differenceLargeValue = 300;
        readonly float _damageTextNormalScale = 0.5f;
        readonly float _damageTextBigScale = 1.0f;
        readonly float _recoveryTextScale = 1.0f;
        readonly float _addEnergyTextDrawPositionY = -29.0f;

        RectTransform _textRectTransform = null;
        Vector2 _textDefaultRectAnchorPosition = new Vector2(0, 26.0f);
        Sequence _doSequence = null;

        Subject<UI.ICharacterTextUIView> _onDisappearSubject;

        new void Awake()
        {
            base.Awake();
            _textRectTransform = _textUI.GetComponent<RectTransform>();
        }

        public IObservable<UI.ICharacterTextUIView> Appear(IUIView parent, Vector2 aboveUIPosition, string text)
        {
            _resetDOTweenSequence();
            return _appear(parent, aboveUIPosition, text, _defaultFont, _moveAppearTime);
        }
        public IObservable<UI.ICharacterTextUIView> AppearHPDifference(IUIView parent, Vector2 aboveUIPosition, int difference)
        {
            _resetDOTweenSequence();
            var isDamage = difference <= 0;
            var textScale = _recoveryTextScale;
            var appearTime = _moveAppearTime;
            if (isDamage)
            {
                // TODO
                // 一定以上のダメージの時にフォントの大きさ(Scale)を変更する
                // 拡大する一定値及び拡大時のScaleについては未確定のため現状はダミーを入れる
                // 初期サイズは通常の半分
                if (System.Math.Abs(difference) >= _differenceLargeValue)
                {
                    textScale = _damageTextBigScale;
                }
                else
                {
                    textScale = _damageTextNormalScale;
                }
                _textRectTransform.localScale = new Vector3(textScale * 0.5f, textScale * 0.5f, 1.0f);
                var endScale = new Vector3(textScale, textScale, 1.0f);
                // そこから元のサイズまで拡大演出
                _doSequence.Insert(0, _textRectTransform.DOScale(endScale, _scaleAppearTime).SetEase(Ease.InQuad));
                appearTime += _scaleAppearTime;
            }
            else
            {
                _textRectTransform.localScale = new Vector3(textScale, textScale, 1.0f);
            }
            _textRectTransform.anchoredPosition = _textDefaultRectAnchorPosition;
            var differenceText = isDamage ? (difference * -1).ToString() : "+" + difference.ToString();
            return _appear(parent, aboveUIPosition, differenceText, isDamage ? _hpDamageFont : _hpRecoveryFont, appearTime);
        }
        public IObservable<UI.ICharacterTextUIView> AppearEnergyDifference(IUIView parent, Vector2 aboveUIPosition, int difference)
        {
            _resetDOTweenSequence();
            if (difference < 0) return Observable.Return<UI.ICharacterTextUIView>(this);
            _textRectTransform.localScale = new Vector3(_recoveryTextScale, _recoveryTextScale, 1.0f);
            _textRectTransform.anchoredPosition = new Vector2(_textDefaultRectAnchorPosition.x, _textDefaultRectAnchorPosition.y + _addEnergyTextDrawPositionY);
            return _appear(parent, aboveUIPosition, "+" + difference.ToString(), _energyRecoveryFont, _moveAppearTime);
        }

        IObservable<UI.ICharacterTextUIView> _appear(IUIView parent, Vector2 aboveUIPosition, string text, Font font, float appearTime)
        {
            SetActive(true);
            var onDisappearSubject = new Subject<UI.ICharacterTextUIView>();

            SetParent(parent, false);
            _transform.localPosition = aboveUIPosition;
            var endPosition = new Vector3(aboveUIPosition.x, aboveUIPosition.y + _textAppearY, 0.0f);
            _doSequence.Append(_transform.DOLocalMove(endPosition, appearTime + _moveDisappearTime));
            _doSequence.OnComplete(() =>
            {
                onDisappearSubject.OnNext((UI.ICharacterTextUIView)this);
                onDisappearSubject.OnCompleted();
                SetActive(false);
            });

            _doSequence.Play();
            _textUI.text = text;
            _textUI.font = font;
            DOTween.To(() => _textUI.color.a, (alpha) => _textUI.color = new Color(_textUI.color.r, _textUI.color.g, _textUI.color.b, alpha), 1, appearTime);
            DOTween.To(() => _textUI.color.a, (alpha) => _textUI.color = new Color(_textUI.color.r, _textUI.color.g, _textUI.color.b, alpha), 0, _moveDisappearTime).SetDelay(appearTime);

            _onDisappearSubject = onDisappearSubject;
            return onDisappearSubject.AsObservable();
        }

        void OnDestroy()
        {
            _clearDOTweenSequence();
        }

        void _clearDOTweenSequence()
        {
            if (_doSequence != null) 
            {
                _doSequence.Complete();
                _doSequence = null;
            }
        }

        void _resetDOTweenSequence()
        {
            _clearDOTweenSequence();
            _doSequence = DOTween.Sequence();
        }
    }
}