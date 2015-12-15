using System;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public class Bullet : ViewModel<IBulletView>
    {
        /// ヒットとする際の水平方向の許容差サイズ
        const int HITTABLE_VERTICAL_SIZE = 4;
        readonly IActionExecutor _actionExecutor;
        readonly uint _characterId;
        int _verticalIndex;
        CompositeDisposable _disposables = new CompositeDisposable();
        bool _isPause = false;

        public Bullet(IActionExecutor actionExecutor, uint characterId)
        {
            _isPause = false;
            _actionExecutor = actionExecutor;
            _characterId = characterId;
        }

        protected override IObservable<IBulletView> _loadView()
        {
            return SceneContainer.GetCacheManager().GetBulletCache(_characterId).Get()
            .Do(v =>
            {
                v.SetActive(true);
                v.SetSkin(_actionExecutor.SkinLevel);
            });
        }

        public IObservable<Bullet> GetOnStock()
        {
            return _view.GetOnStock().Select(v => {
                _disposables.Dispose();
                SceneContainer.GetCacheManager().GetBulletCache(_characterId).Stock(_view);
                return this;
            });
        }

        public IObservable<IActionTargetView> Launch(IActionTarget target, string animationName)
        {
            var onHitObservable = _view.Launch(_actionExecutor.GetViewAsActionExecutorView(), target.GetActionTargetView(), animationName);

            _verticalIndex = _actionExecutor.GetGridPosition().Value.VerticalIndex;
            var verticalDiff = (target.GetGridPosition().Value.VerticalIndex - _actionExecutor.GetGridPosition().Value.VerticalIndex);
            
            if (verticalDiff != 0)
            {
                var estimatedDuration = _view.GetEstimateDuration();
                var timeSpan = estimatedDuration / Math.Abs(verticalDiff);
                SceneContainer.GetTimeHandler().Interval(timeSpan)
                .Subscribe(_ =>
                {
                    if (!_isPause) _verticalIndex += verticalDiff / Math.Abs(verticalDiff);
                }).AddTo(_disposables);
            }
            return onHitObservable;
        }
        public void PauseOn()
        {
            _isPause = true;
            _view.PauseOn();
        }
        public void PauseOff()
        {
            _isPause = false;
            _view.PauseOff();
        }

        public bool IsHittable(int targetVerticalIndex)
        {
            return Math.Abs(targetVerticalIndex - _verticalIndex) <= HITTABLE_VERTICAL_SIZE;
        } 
        public void Stock()
        {
            _view.Stock();
        }
    }
}