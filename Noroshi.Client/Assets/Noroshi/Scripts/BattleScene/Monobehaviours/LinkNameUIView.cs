using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class LinkNameUIView : UIView
    {
        Image _linkImage;
        Text _linkNameText;
        Vector3 _initialLocalPosition;

        new void Awake()
        {
            base.Awake();
            _linkImage = GetComponentInChildren<Image>();
            _linkNameText = GetComponentInChildren<Text>();
            SetActive(false);
        }
        
        public IObservable<bool> Activate(string linkName, float duration, float activeTime, float delay)
        {
            _linkNameText.text = linkName;
            SetActive(true);
            _rectTransform = _linkImage.GetComponent<RectTransform>();
            _rectTransform.localPosition = _rectTransform.localPosition - Vector3.right * _rectTransform.rect.width;
            _initialLocalPosition = _rectTransform.localPosition;
            var onComplete = new Subject<bool>();
            _rectTransform.DOLocalMoveX(_rectTransform.rect.width / 2, duration).SetDelay(delay).OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable().SelectMany(_ => _deactivate(duration, activeTime));
        }
        IObservable<bool> _deactivate(float duration, float delay)
        {
            var onComplete = new Subject<bool>();
            _rectTransform.DOLocalMoveX(_initialLocalPosition.x, duration).SetDelay(delay).OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable();
        }
    }
}
