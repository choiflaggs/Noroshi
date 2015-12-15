using UnityEngine;
using UniRx;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class DropItemUIView : UIView, IDropItemUIView
    {
        [SerializeField] Animator _treasureBoxAnimator = null;

        const string CHANGE_STATE_TREASURE_TAP_NAME = "TreasureTap";

        Subject<bool> _onPickUpSubject = new Subject<bool>();
        Subject<IDropItemUIView> _onGainSubjet = new Subject<IDropItemUIView>();

        public IObservable<bool> Drop(ICharacterView characterView, byte dropItemNo)
        {
            _transform.position = characterView.GetPosition();
            return _onPickUpSubject.AsObservable();
        }
        // UI からコールされる。
        public void PickUp()
        {
            _onPickUpSubject.OnNext(true);
            _onPickUpSubject.OnCompleted();
        }

        // 出現した宝箱のタップ後アニメーション (Treasure_Animation_Tap)が再生終了と同時に呼ばれる
        public void TreasureTapAnimationEnd()
        {
            _treasureBoxAnimator.SetBool(CHANGE_STATE_TREASURE_TAP_NAME, false);
            _onGainSubjet.OnNext(this);
            _onGainSubjet.OnCompleted();
        }

        public IObservable<IDropItemUIView> Gain()
        {
            _treasureBoxAnimator.SetBool(CHANGE_STATE_TREASURE_TAP_NAME, true);
            return _onGainSubjet.AsObservable();
        }
    }
}
