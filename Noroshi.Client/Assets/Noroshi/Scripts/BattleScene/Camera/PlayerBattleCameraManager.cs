using UniRx;

namespace Noroshi.BattleScene.Camera
{
    public class PlayerBattleCameraManager : AbstractCameraManager
    {
        public override void Initialize()
        {
            base.Initialize();

            // Ready 状態に入ったらアニメーション再生時間だけ待ってズームアウト。
            SceneContainer.GetSceneManager().GetOnEnterReadyObservable()
            .SelectMany(_ => SceneContainer.GetTimeHandler().Timer(Constant.BATTLE_READY_ANIMATION_TIME))
            .SelectMany(_ => _cameraController.ZoomOut())
            .Subscribe().AddTo(_disposables);
        }

        public override void Prepare()
        {
            _cameraController.FirstZoomIn();
        }
    }
}
