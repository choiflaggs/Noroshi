using Vector2 = UnityEngine.Vector2;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public interface IUIView : IMonoBehaviour
    {
        Vector2 GetPosition();
        Vector2 GetLocalPosition();
        void SetPosition(Vector2 position);
        void SetLocalPosition(Vector2 localPosition);
    }
}
