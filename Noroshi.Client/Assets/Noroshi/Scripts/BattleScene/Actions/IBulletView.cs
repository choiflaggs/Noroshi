using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public interface IBulletView : MonoBehaviours.IView
    {
        /// ヒットする度にヒットした対象がやってくる Observable を取得。該当 GameObject 破棄時に Complate となる。
        IObservable<IActionTargetView> Launch(IActionExecutorView iExecutorView, IActionTargetView iTargetView, string animationName);
        float GetEstimateDuration();
        IObservable<IBulletView> GetOnStock();
        void SetSkin(int skinLevel);
        void Stock();
        void PauseOn();
        void PauseOff();
    }
}