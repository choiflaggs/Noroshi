using Vector2 = UnityEngine.Vector2;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public interface IView : IMonoBehaviour
    {
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
    }
}
