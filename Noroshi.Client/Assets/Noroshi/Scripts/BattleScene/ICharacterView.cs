using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;
using UniRx;
using Noroshi.Grid;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene
{
    public enum PositionInCharacterView
    {
        Bottom = 1,
        Center = 2,
        Top = 3,
        Front = 4,
    }
    public enum CenterPositionForUIType
    {
        Story = 1,
        Result = 2,
    }

    public interface ICharacterView : MonoBehaviours.IView, Actions.IActionExecutorView
    {
        IObservable<bool> GetOnExitActionAnimationObservable();
        IObservable<bool> GetOnInvokeActionObservable();
        IObservable<bool> GetOnExitTimeStopObservable();
        IObservable<bool> GetOnExitWinAnimationObservable();
        IObservable<BulletHitEvent> GetOnHitObservable();
        IEnumerable<ActionAnimation> GetActionAnimations();

        void PauseOn();
        void PauseOff();
        void SetSpeed(float speed);

        IObservable<bool> Move(Vector2 position, float duration);
        IObservable<bool> Move(IEnumerable<Vector2> positions, float duration);
        void StopMove();
        void PlayIdle();
        void PlayWalk();
        IObservable<bool> WalkTo(Vector2 position, float duration);
        IObservable<bool> RunTo(Vector2 position, float duration);
        void PlayAction(string actionAnimationName);
        void PlayDamage();
        void PlayKnockback(Vector2 position, float duration);
        void PlayStun();
        void PlayVanish();
        IObservable<bool> PlayAnimationAtEnterDead();
        IObservable<bool> PlayEscape();
        void PlayWin();

        void Resurrect();

        void SetSkin(byte skinLevel);
        void SetHorizontalDirection(Direction direction);
        void SetOrderInLayer(int orderInLayer);

        void SetEscapeBeforeDeadOn();
        void PlayEscapeBeforeDead();

        IActionTargetView GetActionTargetView();
        void SetUpForUI(CenterPositionForUIType centerPositionForUIType);
    }
}
