using System.Collections.Generic;
using UniRx;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.BattleScene.Actions
{
    public class IndirectRangeAttack : AbstractRangeAttack
    {
        List<Bullet> _bullets = new List<Bullet>();
        public IndirectRangeAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override RelativeForce? _targetRelativeForce { get { return _arg1 > 0 ? (RelativeForce?)_arg1 : null; } }
        public override int MinRange { get { return _arg2; } }
        public override int MaxRange { get { return _arg3; } }
        protected override float? _damageCoefficient { get { return _arg4 > 0 ? (float?)_arg4 / 100.0f : null; } }

        public override IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory)
        {
            return SceneContainer.GetCacheManager().PreloadBulletView(factory, new uint[]{executor.CharacterID})
            .Select(_ => (IAction)this);
        }

        public override void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets)
        {
            // ターゲットのヒットに処理を紐ずける
            CompositeDisposable _disposables = new CompositeDisposable();
            foreach (var target in targets)
            {
                target.GetOnHitObservable().Where(bulletHitEvent => bulletHitEvent.Action == this)
                .Subscribe(bulletHitEvent => {
                    var hitTarget = bulletHitEvent.ActionTarget;
                    if (!hitTarget.IsTargetable || !bulletHitEvent.Bullet.IsHittable(hitTarget.GetGridPosition().Value.VerticalIndex)) return;
                    OnHit(executor, bulletHitEvent.ActionTarget);
                    _bullets.Remove(bulletHitEvent.Bullet);
                    if (!_canPenetrate) bulletHitEvent.Bullet.Stock();
                }).AddTo(_disposables);
            }
            // Bullet 発射処理。Complete されるので Dispose は不要。
            Observable.WhenAll(_filterTargets(targets).Select(target =>
            {
                var bullet = new Bullet(executor, executor.CharacterID);
                _bullets.Add(bullet);
                return bullet.LoadView()
                .Do(_ => bullet.GetOnStock().Subscribe())
                .SelectMany(v => bullet.Launch(target, AnimationName).Do(atv =>
                {
                    atv.Hit(new BulletHitEvent(){Bullet = bullet, Action = this});
                }));
            }))
            .Subscribe(_ => {}, () => _disposables.Dispose());
        }

        public override void Reset(IActionExecutor executor)
        {
            foreach (var bullet in _bullets.ToArray())
            {
                bullet.Stock();
                _bullets.Remove(bullet);
            }
        }
        public override void EnterTimeStop()
        {
            foreach (var bullet in _bullets)
            {
                bullet.PauseOn();
            }
        }
        public override void ExitTimeStop()
        {
            foreach (var bullet in _bullets)
            {
                bullet.PauseOff();
            }
        }

        void OnHit(IActionExecutor executor, IActionTarget target)
        {
            _attackable.Attack(executor, target);
        }

        // TODO
        bool _canPenetrate { get { return false; } }
    }
}