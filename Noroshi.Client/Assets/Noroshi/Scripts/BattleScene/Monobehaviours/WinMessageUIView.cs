using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class WinMessageUIView : UIView
    {
        [SerializeField] WinMessageAnimatorUIView _winMessageBackgroundAnimator = null;
        [SerializeField] WinMessageAnimatorUIView _winMessageTextImageAnimator = null;

        UnityEngine.UI.Image _winMessageBackground = null;
        UnityEngine.UI.Image _winMessageTextImage = null;

        void Start()
        {
            _winMessageTextImage = _winMessageTextImageAnimator.WinMessageImage;
            _winMessageBackground = _winMessageBackgroundAnimator.WinMessageImage;
        }

        public IObservable<bool> PlayWinMessageAnimation()
        {
            _winMessageTextImage.enabled = true;
            _winMessageBackground.enabled = true;

            var allWinMessageAnimator = Observable.WhenAll<bool>(
                _winMessageBackgroundAnimator.PlayWinMessageAnimation(),
                _winMessageTextImageAnimator.PlayWinMessageAnimation()).Select(_ => true);

            return allWinMessageAnimator.SelectMany(allWinMessageAnimator)
            .Do(_ => 
            {
                _winMessageBackground.enabled = false;
                _winMessageTextImage.enabled = false;
            })
            .Select(_ => true);
        }
    }
}