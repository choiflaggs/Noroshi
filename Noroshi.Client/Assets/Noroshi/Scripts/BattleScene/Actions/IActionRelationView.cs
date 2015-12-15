namespace Noroshi.BattleScene.Actions
{
    public interface IActionRelationView
    {
        void Appear(IActionExecutorView executorView, IActionTargetView targetView, float duration);
        void Disappear();
        void PauseOn();
        void PauseOff();
    }
}