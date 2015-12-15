using UniRx;

namespace Noroshi.BattleScene.Camera
{
    public interface ICameraController
    {
        void FirstZoomIn();
        IObservable<bool> ZoomOut();
        IObservable<bool> MoveX(float x, float duration);
        IObservable<bool> Shake();
        IObservable<bool> ShakeByAction();
        void ClearShakeByAction();
        void EnterTimeStop();
        void ExitTimeStop();
    }
}