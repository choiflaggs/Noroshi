using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IModalUIView : MonoBehaviours.IUIView
    {
        /// クローズイベント用 Observable を取得するためのメソッド。
        IObservable<bool> GetClickCloseObservable();
        /// モーダルを開くメソッド。
        IObservable<IModalUIView> Open();
        /// モーダルを閉じるメソッド。
        IObservable<IModalUIView> Close();
    }
}
