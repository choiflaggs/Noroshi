using Vector2 = UnityEngine.Vector2;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionTargetView
    {
        void Hit(BulletHitEvent bulletHitEvent);
        Vector2 GetPosition();
    }
}