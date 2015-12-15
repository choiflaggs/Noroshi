using UniRx;
using UnityEngine;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class DarkScreenUIView : UIView
    {
        UnityEngine.UI.Image _image;

        new void Awake()
        {
            base.Awake();
            _image = GetComponent<UnityEngine.UI.Image>();
            SetActive(false);
        }

        public IObservable<DarkScreenUIView> Activate(float alpha, float duration)
        {
            SetActive(true);
            var onActivate = new Subject<DarkScreenUIView>();
            _image.color = new Color(0, 0, 0, 0);
            _image.DOColor(new Color(0, 0, 0, alpha), duration).OnComplete(() =>
            {
                onActivate.OnNext(this);
                onActivate.OnCompleted();
            });
            return onActivate.AsObservable();
        }

        public IObservable<DarkScreenUIView> Deactivate(float duration)
        {
            var onDeactivate = new Subject<DarkScreenUIView>();
            _image.DOColor(new Color(0, 0, 0, 0), duration).OnComplete(() =>
            {
                SetActive(false);
                onDeactivate.OnNext(this);
                onDeactivate.OnCompleted();
            });
            return onDeactivate.AsObservable();
        }
    }
}
