using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class PauseModalUIView : ModalUIView, UI.IPauseModalUIView
    {
        Subject<bool> _onClickWithdrawalSubject = new Subject<bool>();

        public IObservable<bool> GetOnClickWithdrawalObservable()
        {
            return _onClickWithdrawalSubject.AsObservable();
        }

        /// UnityEngine.UI.Button にセットするためのメソッド。
        /// 撤退用のイベントをプッシュする
        public void ClickWithdrawal()
        {
            _onClickWithdrawalSubject.OnNext(true);
        }
    }
}
