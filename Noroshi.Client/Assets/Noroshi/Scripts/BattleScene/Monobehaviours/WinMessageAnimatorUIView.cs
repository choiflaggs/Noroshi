using UnityEngine;
using UniRx;


namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class WinMessageAnimatorUIView : UIView
    {
        Animator _winMessageAnimator = null;
        UnityEngine.UI.Image _winMessageImage = null;

        Subject<bool> _onWinAnimationEndSubject = new Subject<bool>();

        public UnityEngine.UI.Image WinMessageImage { get { return _winMessageImage; } }

        new void Awake()
        {
            base.Awake();
            _winMessageAnimator = GetComponent<Animator>();
            _winMessageImage = GetComponent<UnityEngine.UI.Image>();
            _winMessageImage.enabled = false;
        }

        void OnDestroy()
        {
            _onWinAnimationEndSubject.OnCompleted();
        }

        public IObservable<bool> PlayWinMessageAnimation()
        {
            _winMessageAnimator.SetBool("Start", true);
            return _onWinAnimationEndSubject.AsObservable();
        }

        // WinMessage関連のアニメーションが終了した時に呼ばれる
        public void OnWinAnimationEnd()
        {
            _winMessageAnimator.SetBool("Start", false);
            _onWinAnimationEndSubject.OnNext(true);
            _onWinAnimationEndSubject.OnCompleted();
        }
    }
}