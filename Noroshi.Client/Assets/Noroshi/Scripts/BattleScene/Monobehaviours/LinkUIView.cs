using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class LinkUIView : UIView, ILinkUIView
    {
        [SerializeField] Text _linkNumText;
        [SerializeField] float _linkSlideDuration = 0.25f;
        [SerializeField] float _linkActiveTime = 2f;
        [SerializeField] float _delayPerLinkSlide = 0.1f;

        LinkNameUIView[] _linkNameUIViews;

        new void Awake()
        {
            base.Awake();
            _linkNameUIViews = GetComponentsInChildren<LinkNameUIView>();
            SetActive(false);
        }

        public IObservable<ILinkUIView> Activate(string[] linkNames)
        {
            _linkNumText.text = linkNames.Length.ToString();
            SetActive(true);
            List<IObservable<bool>> observables = new List<IObservable<bool>>();
            var maxNum = Mathf.Min(linkNames.Length, _linkNameUIViews.Length);
            for (var i = 0; i < maxNum; i++)
            {
                observables.Add(_linkNameUIViews[i].Activate(linkNames[i], _linkSlideDuration, _linkActiveTime, _delayPerLinkSlide * i));
            }
            return Observable.WhenAll(observables).SelectMany(_deactivate()).Select(_ => (ILinkUIView)this);
        }

        IObservable<bool> _deactivate()
        {
            return Observable.Return<bool>(true).Do(_ => SetActive(false));
        }
    }
}
