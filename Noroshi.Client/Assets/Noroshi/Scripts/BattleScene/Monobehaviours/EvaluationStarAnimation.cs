using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(Animator))]
    public class EvaluationStarAnimation : UIView 
    {
        Animator _evaluationAnimator;
        Subject<EvaluationStarAnimation> _onAnimationEvent = new Subject<EvaluationStarAnimation>();

        new void Awake()
        {
            base.Awake();
            _evaluationAnimator = GetComponent<Animator>();
        }

        void OnDestroy()
        {
            _onAnimationEvent.OnCompleted();
        }

        public IObservable<EvaluationStarAnimation> PlayAnimation()
        {
            _evaluationAnimator.SetBool("Start", true);
            return _onAnimationEvent.AsObservable();
        }

        // Animationに仕込んだ関数
        // 画像の縮小アニメーション終了時に呼ばれる
        public void OnAnimatinoEvent()
        {
            _onAnimationEvent.OnNext(this);
        }
    }
}