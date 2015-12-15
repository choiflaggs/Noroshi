using UnityEngine;

namespace Noroshi.BattleScene.MonoBehaviours
{
    /// 最低限の機能拡張した MonoBehaviour の子クラス。IF 対応も。
    public class MonoBehaviour : UnityEngine.MonoBehaviour, IMonoBehaviour
    {
        protected GameObject _gameObject;
        protected Transform _transform;

        protected void Awake()
        {
            _gameObject = gameObject;
            _transform = transform;
        }

        public virtual bool SetActive(bool active)
        {
            var res = _gameObject.activeSelf != active;
            _gameObject.SetActive(active);
            return res;
        }

        public virtual void SetParent(IMonoBehaviour parent, bool worldPositionStays)
        {
            var parentMonoBehaviour = (MonoBehaviour)parent;
            _transform.SetParent(parentMonoBehaviour.GetTransform(), worldPositionStays);
        }
        public virtual void RemoveParent()
        {
            _transform.parent = null;
        }

        public virtual void Dispose()
        {
            Destroy(_gameObject);
        }

        public Transform GetTransform()
        {
            return _transform;
        }
    }
}
