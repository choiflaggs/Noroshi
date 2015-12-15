using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [ExecuteInEditMode]
    public class ImageSquareMoveUI : MonoBehaviours.MonoBehaviour
    {
        enum MovePositions
        {
            RightUp = 0,
            LeftUp,
            LeftDown,
            RightDown,

            Max
        }

        [SerializeField] Image _targetImage;
        [SerializeField] RectTransform _movingRange;
        [SerializeField] MovePositions _firstNextPosition;

        int _nextMovePosition
        {
            set
            {
                _firstNextPosition = (MovePositions)value;
                if (_firstNextPosition >= MovePositions.Max) _firstNextPosition = MovePositions.RightUp;
            }
            get
            {
                return (int)_firstNextPosition;
            }
        }

        float _heightBuf = 0.0f;
        Sequence _sequence = null;
        RectTransform _targetTransform = null;
        Vector2[] _endPositions = null;

        new void Awake()
        {
            base.Awake();
            _sequence = DOTween.Sequence();
            _seupData();
        }

        void _setupEndPositions()
        {
            if (_movingRange == null) return;
            if ( _endPositions == null || _endPositions.Length == 0) _endPositions = new Vector2[(int)MovePositions.Max];
            var rect = _movingRange.rect;
            _endPositions[(int)MovePositions.RightUp]   = new Vector2(rect.xMax - _heightBuf, rect.yMax - _heightBuf);
            _endPositions[(int)MovePositions.LeftUp]    = new Vector2(rect.x    + _heightBuf, rect.yMax - _heightBuf);
            _endPositions[(int)MovePositions.LeftDown]  = new Vector2(rect.x    + _heightBuf, rect.y    + _heightBuf);
            _endPositions[(int)MovePositions.RightDown] = new Vector2(rect.xMax - _heightBuf, rect.y    + _heightBuf);
        }

        void _setTweenData()
        {
            for (byte i = 0; i < 4; i++)
            {
                var type = (i % 2);
                _sequence.Append(_targetTransform.DOAnchorPos(_endPositions[_nextMovePosition], 0.5f).SetEase((type == 0) ? Ease.InQuad : Ease.OutQuad));
                _sequence.Append(_targetTransform.DOLocalRotate(_getEulerAngle(), 0.01f));
            }
            _sequence.SetLoops(-1, LoopType.Restart);

            _sequence.Pause();
        }

        Vector3 _getEulerAngle()
        {
            var prevPosition = _nextMovePosition;
            _nextMovePosition = _nextMovePosition + 1;
            var sub = (_endPositions[_nextMovePosition] - _endPositions[prevPosition]).normalized;
            return new Vector3(0, 0, _getAngle(sub));
        }

        float _getAngle(Vector2 vector)
        {
            if (vector.x < 0.0f && vector.y == 0.0f) return 180.0f;
            if (vector.x == 0.0f && vector.y < 0.0f) return -90.0f;
            if (vector.x > 0.0f && vector.y == 0.0f) return 0.0f;
            if (vector.x == 0.0f && vector.y > 0.0f) return 90.0f;

            return 0.0f;
        }

        void _setImageData()
        {
            if (_targetImage != null) _targetTransform = _targetImage.rectTransform;
            if (_targetImage != null) _heightBuf = _targetImage.sprite.rect.height / 2;
        }

        void _seupData()
        {
            _setImageData();
            _setupEndPositions();
            _setTweenData();
        }

        public void AnimationPlay()
        {
            _sequence.Play();
        }

        public void AnimationPause()
        {
            _sequence.Pause();
        }

        public override void Dispose ()
        {
            base.Dispose ();
            _sequence.Kill();
        }
    }
}
