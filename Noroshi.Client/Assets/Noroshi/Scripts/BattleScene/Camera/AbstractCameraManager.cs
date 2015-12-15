using UniRx;
using System;

namespace Noroshi.BattleScene.Camera
{
    public abstract class AbstractCameraManager : ICameraManager
    {
        protected ICameraController _cameraController;
        protected CompositeDisposable _disposables = new CompositeDisposable();

        public virtual void Initialize()
        {
            SceneContainer.GetBattleManager().GetOnCompletePrepareNextWaveObservable()
            .Subscribe(no => _moveCamera(no)).AddTo(_disposables);

            SceneContainer.GetBattleManager().GetOnFinishBattleObservable()
            .SelectMany(_ => _onFinishBattle())
            .Subscribe().AddTo(_disposables);

            SceneContainer.GetBattleManager().GetOnTryCameraShakeObservable()
            .SelectMany(cameraSyakeByActionType => _shakeCamera(cameraSyakeByActionType))
            .Subscribe().AddTo(_disposables);
        }

        public IObservable<IManager> LoadDatas()
        {
            return Observable.Return<IManager>(this);
        }

        public IObservable<IManager> LoadAssets(IFactory factory)
        {
            return factory.BuildCameraController().Select(c =>
            {
                _cameraController = c;
                return (IManager)this;
            });
        }
        public virtual void Prepare()
        {
        }

        protected IObservable<bool> _moveCamera(byte waveNo)
        {
            return _cameraController.MoveX(WaveField.GetPositionX(waveNo), SceneContainer.GetBattleManager().GetSwitchWaveTime());
        }

        IObservable<bool> _onFinishBattle()
        {
            _cameraController.ClearShakeByAction();
            return _cameraController.Shake();
        }

        IObservable<bool> _shakeCamera(Noroshi.BattleScene.Actions.CameraShakeByActionType cameraShakeByActionType)
        {
            switch (cameraShakeByActionType)
            {
            case Noroshi.BattleScene.Actions.CameraShakeByActionType.Play:
                _cameraController.ClearShakeByAction();
                return _cameraController.ShakeByAction();
            case Noroshi.BattleScene.Actions.CameraShakeByActionType.Pause:
                _cameraController.EnterTimeStop();
                return Observable.Return<bool>(true);
            case Noroshi.BattleScene.Actions.CameraShakeByActionType.Replay:
                _cameraController.ExitTimeStop();
                return Observable.Return<bool>(true);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
