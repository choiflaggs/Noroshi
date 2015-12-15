using UniRx;
using UniLinq;
using Noroshi.BattleScene.Actions.Roles;
using System.Collections.Generic;

namespace Noroshi.BattleScene.Actions
{
    public class IndirectAttack : AbstractNoRangeAttack
    {
        List<Bullet> _bullets = new List<Bullet>();
        public IndirectAttack(Core.WebApi.Response.Character.Action data) : base(data)
        {
        }
        protected override TargetSelectType? _targetSelectType { get { return _arg1 > 0 ? (TargetSelectType?)_arg1 : null; } }
        protected override int? _targetTagFlags { get { return _arg2 > 0 ? (int?)_arg2 : null; } }
        protected override float? _damageCoefficient { get { return _arg5 > 0 ? (float?)_arg5 / 100.0f : null; } }

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
            Observable.WhenAll(_noRangeSearchable.FilterTargets(actionTargetFinder, targets).Select(target =>
            {
                var bullet = new Bullet(executor, executor.CharacterID);
                _bullets.Add(bullet);
                return bullet.LoadView()
                .Do(_ => bullet.GetOnStock().Subscribe())
                .SelectMany(v => bullet.Launch(target, AnimationName).Do(atv => {
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