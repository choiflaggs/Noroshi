using UnityEngine;
using UniRx;
using Noroshi.BattleScene.Camera;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] UnityEngine.Camera _mainCamera;
        [SerializeField] float _zoomRatio = 1.75f;
        [SerializeField] float _shakeDuration = 0.5f;
        [SerializeField] Vector3 _shakeStrength = new Vector3(0.5f, 1.0f, 0);
        [SerializeField] int _shakeVibrato = 10;

        float _mainCameraOriginalSize;
        Transform _mainCameraTransform;
        Sequence _doTweenSequence = null;

        void Start()
        {
            _mainCameraTransform = _mainCamera.transform;
            _mainCameraOriginalSize = _mainCamera.orthographicSize;
        }

        public void FirstZoomIn()
        {
            _mainCamera.orthographicSize = _mainCameraOriginalSize / _zoomRatio;
        }

        public IObservable<bool> ZoomOut()
        {
            var zoomOutDuration = Constant.BATTLE_READY_ZOOM_OUT_TIME;
            var onCompleteSubject = new Subject<bool>();
            _mainCamera.DOOrthoSize(_mainCameraOriginalSize, zoomOutDuration).OnComplete(() =>
            {
                onCompleteSubject.OnNext(true);
                onCompleteSubject.OnCompleted();
            });
            return onCompleteSubject.AsObservable();
        }

        public IObservable<bool> MoveX(float x, float duration)
        {
            var onCompleteSubject = new Subject<bool>();
            _mainCameraTransform.DOMoveX(x, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                onCompleteSubject.OnNext(true);
                onCompleteSubject.OnCompleted();
            });
            return onCompleteSubject.AsObservable();
        }

        public IObservable<bool> Shake()
        {
            var onShakeCompleteSubject = new Subject<bool>();
            _mainCamera.DOShakePosition(_shakeDuration, _shakeStrength, _shakeVibrato, 0).OnComplete(() =>
            {
                onShakeCompleteSubject.OnNext(true);
                onShakeCompleteSubject.OnCompleted();
            });
            return onShakeCompleteSubject.AsObservable();
        }

        public IObservable<bool> ShakeByAction()
        {
            _doTweenSequence = DOTween.Sequence();
            var onShakeCompleteSubject = new Subject<bool>();

            var duration = 0.05f;
            _doTweenSequence.Append(_mainCameraTransform.DOMoveY(-0.5f, duration));
            _doTweenSequence.Append(_mainCameraTransform.DOMoveY(0.5f, duration));
            _doTweenSequence.Append(_mainCameraTransform.DOMoveY(-0.25f, duration));
            _doTweenSequence.Append(_mainCameraTransform.DOMoveY(0.25f, duration));
            _doTweenSequence.Append(_mainCameraTransform.DOMoveY(0.0f, duration));

            _doTweenSequence.OnComplete(() => 
            {
                onShakeCompleteSubject.OnNext(true);
                onShakeCompleteSubject.OnCompleted();
                _doTweenSequence = null;
            });

            _doTweenSequence.Play();
            return onShakeCompleteSubject.AsObservable();
        }

        public void EnterTimeStop()
        {
            if (_doTweenSequence != null) _doTweenSequence.Pause();
        }

        public void ExitTimeStop()
        {
            if (_doTweenSequence != null) _doTweenSequence.Play();
        }

        public void ClearShakeByAction()
        {
            _doTweenSequence.Complete();
            _doTweenSequence = null;
        }

        public void AddChildToMainCamera(Transform transform)
        {
            transform.SetParent(_mainCameraTransform);
        }
    }
}