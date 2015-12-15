using UniRx;
using Vector2 = UnityEngine.Vector2;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionView
    {
        void Appear(Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder);
        void Appear(Vector2 position, Grid.Direction direction, string animationName, int sortingOrder);
        IObservable<IActionView> Launch(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder);
        IObservable<IActionView> Launch(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder, byte? animationSequenceNumber);
        void Disappear();
        void SetSkin(int skinLevel);
        void PauseOn();
        void PauseOff();
    }
}