using DG.Tweening;
using UnityEngine;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public enum MoveType
    {
        Parabola = 1,
        Linear = 2,
    }
    
    public enum PositionType
    {
        Target = 1,
        ScreenCenter = 2,
        ScreenUpperPosition = 3,
    }

    [System.Serializable]
    public class AsynchronousMoveParam
    {
        [SerializeField] bool _useAngle = true;
        [SerializeField] MoveType _moveType = MoveType.Parabola;
        [SerializeField] float _duration = 1.0f;
        [SerializeField] PositionType _positionType = PositionType.Target;
        [SerializeField] float _upperPosition = 0.0f;
        [SerializeField] Ease _horizontalEasing = Ease.Linear;
        [SerializeField] Ease _verticalEasing = Ease.Linear;
        [SerializeField] Ease _verticalInEasing = Ease.Linear;
        [SerializeField] Ease _verticalOutEasing = Ease.Linear;
        [SerializeField] SortingLayerSetting[] _sortingLayerSettings;

        public bool UseAngle { get { return _useAngle; } }
        public MoveType MoveType { get { return _moveType; } }
        public float Duration { get { return _duration; } }
        public PositionType PositionType { get { return _positionType; } }
        public float UpperPosition { get { return _upperPosition; } }
        public Ease HorizontalEasing { get { return _horizontalEasing; } }
        public Ease VerticalEasing { get { return _verticalEasing;} }
        public Ease VerticalInEasing { get { return _verticalInEasing;} }
        public Ease VerticalOutEasing { get { return _verticalOutEasing;} }

        public void SetSortingLayer(int sortingOrder)
        {
            foreach (var sortingLayerSetting in _sortingLayerSettings)
            {
                sortingLayerSetting.SetSortingLayer(sortingOrder);
            }
        }

        [System.Serializable]
        class SortingLayerSetting
        {
            [SerializeField] MeshRenderer _meshRenderer = null;
            [SerializeField] string _layerName = "Default";
            [SerializeField] int _relativeSortingOrder = 0;

            public void SetSortingLayer(int sortingOrder)
            {
                _meshRenderer.sortingLayerName = _layerName;
                _meshRenderer.sortingOrder = sortingOrder + _relativeSortingOrder;
            }
        }
    }
}