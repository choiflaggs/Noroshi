using UnityEngine;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class UIView : MonoBehaviour, IUIView
    {
        protected RectTransform _rectTransform = null;

        new void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
        }

        public Vector2 GetPosition()
        {
            return _transform.position;
        }

        public Vector2 GetLocalPosition()
        {
            return _transform.localPosition;
        }

        public void SetPosition(Vector2 position)
        {
            _transform.position = position;
        }

        public void SetLocalPosition(Vector2 localPosition)
        {
            _transform.localPosition = localPosition;
        }

        public RectTransform GetRectTransform()
        {
            return _rectTransform;
        }
    }
}
