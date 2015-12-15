using UnityEngine;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class View : MonoBehaviour, IView
    {
        public Vector2 GetPosition()
        {
            return _transform.position;
        }

        public void SetPosition(Vector2 position)
        {
            _transform.position = position;
        }
    }
}
