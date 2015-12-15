using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(Animator))]
    public class SkillStartingEffectAnimation : UIView
    {
        Animator _skillStartinEffectAnimator = null;

        new void Awake()
        {
            base.Awake();
            _skillStartinEffectAnimator = GetComponent<Animator>();
        }

        public void PlayAnimation()
        {
            _skillStartinEffectAnimator.SetBool("Start", true);
        }

        // アニメーション終了時に呼ばれるコールバック
        public void OnAnimationEnd()
        {
            _skillStartinEffectAnimator.SetBool("Start", false);
        }
    }
}