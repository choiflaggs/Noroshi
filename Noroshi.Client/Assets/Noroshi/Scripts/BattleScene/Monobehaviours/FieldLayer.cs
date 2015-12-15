using UnityEngine;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class FieldLayer : MonoBehaviour
    {
        [SerializeField] float _moveRatio = 0;
        Transform _mainCameraTransform;
        Vector3 _initialPositionDiff;

        void Start()
        {
            _mainCameraTransform = UnityEngine.Camera.main.transform;
            _initialPositionDiff = _transform.position - _mainCameraTransform.position;
        }
        void Update()
        {
            _transform.position = _mainCameraTransform.position * (1 - _moveRatio) + _initialPositionDiff;
        }
    }
}