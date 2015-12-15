using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.MonoBehaviours
{
    /// Action 当たり判定用 View。ヒットしたら該当 Action をプッシュするだけのクラス。
    public class ActionTargetView : View, IActionTargetView
    {
        Subject<BulletHitEvent> _onHitSubject = new Subject<BulletHitEvent>();

        public IObservable<BulletHitEvent> GetOnHitObservable()
        {
            return _onHitSubject.AsObservable();
        }

        public void Hit(BulletHitEvent bulletHitEvent)
        {
            _onHitSubject.OnNext(bulletHitEvent);
        }
    }
}