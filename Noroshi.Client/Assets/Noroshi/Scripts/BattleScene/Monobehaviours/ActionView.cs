using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Noroshi.BattleScene.Actions;
using UniRx;
using UniRx.Triggers;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ActionView : SpineView, IActionView
    {
        [SerializeField] AsynchronousMoveParam _action1AsynchronousMoveParam;
        [SerializeField] AsynchronousMoveParam _action2AsynchronousMoveParam;
        [SerializeField] AsynchronousMoveParam _action3AsynchronousMoveParam;
        [SerializeField] AsynchronousMoveParam _action4AsynchronousMoveParam;
        [SerializeField] AsynchronousMoveParam _action5AsynchronousMoveParam;

        Dictionary<string, AsynchronousMoveParam> _actionAsynchronousMoveParamDictionary = new Dictionary<string, AsynchronousMoveParam>();
        bool _isLauncing = false;

        new void Start()
        {
            base.Start();

            _actionAsynchronousMoveParamDictionary.Add(Constant.ACTION_RANK_0_ANIMATION_NAME, _action1AsynchronousMoveParam);
            _actionAsynchronousMoveParamDictionary.Add(Constant.ACTION_RANK_1_ANIMATION_NAME, _action2AsynchronousMoveParam);
            _actionAsynchronousMoveParamDictionary.Add(Constant.ACTION_RANK_2_ANIMATION_NAME, _action3AsynchronousMoveParam);
            _actionAsynchronousMoveParamDictionary.Add(Constant.ACTION_RANK_3_ANIMATION_NAME, _action4AsynchronousMoveParam);
            _actionAsynchronousMoveParamDictionary.Add(Constant.ACTION_RANK_4_ANIMATION_NAME, _action5AsynchronousMoveParam);

            this.UpdateAsObservable().Scan(_transform.position, (prev, _) =>
            {
                var cur = _transform.position;
                if (_isLauncing)
                {
                    var direction = cur - prev;
                    var angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
                    var eulerAngle = Quaternion.AngleAxis(360.0f - angle, Vector3.forward).eulerAngles;
                    eulerAngle.y = 180;
                    _transform.eulerAngles = eulerAngle;
                }
                return cur;
            })
            .Skip(1)
            .Subscribe()
            .AddTo(this);
        }

        public void Appear(Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder)
        {
            var actionAsynchronousMoveParam = _actionAsynchronousMoveParamDictionary[animationName];
            actionAsynchronousMoveParam.SetSortingLayer(sortingOrder);
            var position = SceneContainer.GetBattleManager().CurrentWave.Field.GetPosition(grid);
            SetPosition(_getAppearPosition(position, actionAsynchronousMoveParam.PositionType, actionAsynchronousMoveParam.UpperPosition));
            SetHorizontalDirection(direction);
            _play(animationName, false);
            SetActive(true);
        }
        public void Appear(Vector2 position, Grid.Direction direction, string animationName, int sortingOrder)
        {
            var actionAsynchronousMoveParam = _actionAsynchronousMoveParamDictionary[animationName];
            actionAsynchronousMoveParam.SetSortingLayer(sortingOrder);
            SetPosition(_getAppearPosition(position, actionAsynchronousMoveParam.PositionType, actionAsynchronousMoveParam.UpperPosition));
            SetHorizontalDirection(direction);
            _play(animationName, false);
            SetActive(true);
        }

        Vector3 _getAppearPosition(Vector3 position, PositionType positionType, float upperPosition)
        {
            var mainCameraPosition = UnityEngine.Camera.main.transform.position;
            switch (positionType)
            {
            case PositionType.Target:
                return position;
            case PositionType.ScreenCenter:
                return new Vector3(mainCameraPosition.x, 0, 0);
            case PositionType.ScreenUpperPosition:
                return new Vector3(mainCameraPosition.x, upperPosition, 0);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        public IObservable<IActionView> Launch(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder, byte? animationSequenceNumber)
        {
            var asynchronousMoveParam = _actionAsynchronousMoveParamDictionary[animationName];
            if (animationSequenceNumber.HasValue) animationName += "_" + animationSequenceNumber.Value.ToString();
            return _launch(executorView, grid, direction, animationName, sortingOrder, asynchronousMoveParam);
        }

        public IObservable<IActionView> Launch(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder)
        {
            var asynchronousMoveParam = _actionAsynchronousMoveParamDictionary[animationName];
            return _launch(executorView, grid, direction, animationName, sortingOrder, asynchronousMoveParam);
        }

        IObservable<IActionView> _launch(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortingOrder, AsynchronousMoveParam asynchronousMoveParam)
        {
            asynchronousMoveParam.SetSortingLayer(sortingOrder);
            var toPosition = SceneContainer.GetBattleManager().CurrentWave.Field.GetPosition(grid);
            _setPosition(executorView, direction);
            _play(animationName + "_attack").Subscribe();
            SetActive(true);
            _isLauncing = asynchronousMoveParam.UseAngle;
            _transform.DOMoveX(toPosition.x, asynchronousMoveParam.Duration).SetEase(asynchronousMoveParam.HorizontalEasing).OnComplete(() => {
            });
            string effectAnimationName = animationName + "_effect";
            switch(asynchronousMoveParam.MoveType)
            {
            case MoveType.Linear:
                return _moveLinear(effectAnimationName, toPosition, asynchronousMoveParam.Duration, asynchronousMoveParam.VerticalEasing);
            case MoveType.Parabola:
                return _moveParabola(effectAnimationName, toPosition, asynchronousMoveParam.Duration, asynchronousMoveParam.VerticalInEasing, asynchronousMoveParam.VerticalOutEasing);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Disappear()
        {
            SetActive(false);
        }
        
        void _setPosition(IActionExecutorView iExecutorView, Grid.Direction direction)
        {
            var view = (SpineView)iExecutorView;
            _transform.position = ((CharacterView)iExecutorView).GetArmPosition();
            SetOrderInLayer(view.GetOrderInLayer());
            SetHorizontalDirection(direction);
        }

        IObservable<IActionView> _moveLinear(string animationName, Vector2 toPosition, float duration, Ease verticalEasing)
        {
            var subject = new Subject<IActionView>();
            _transform.DOMoveY(toPosition.y, duration).SetEase(verticalEasing).OnComplete(() => 
            {
                _isLauncing = false;
                _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
                _resetUnreversibleTransfromRotation();
                _play(animationName, false).Subscribe();
                subject.OnNext((IActionView)this);
                subject.OnCompleted();
            });
            return subject.AsObservable();
        }

        IObservable<IActionView> _moveParabola(string animationName, Vector2 toPosition, float duration, Ease verticalInEasing, Ease verticalOutEasing)
        {
            var subject = new Subject<IActionView>();
            _transform.DOMoveY(toPosition.y + 5f, duration / 2).SetEase(verticalOutEasing).OnComplete(() => {
                _transform.DOMoveY(toPosition.y, duration - duration / 2).SetEase(verticalInEasing).OnComplete(() => {
                    _isLauncing = false;
                    _transform.localEulerAngles = new Vector3(0, _transform.localEulerAngles.y, 0);
                    _resetUnreversibleTransfromRotation();
                    _play(animationName, false).Subscribe();
                    subject.OnNext((IActionView)this);
                    subject.OnCompleted();
                });
            });
            return subject.AsObservable();
        }
        
        public void SetSkin(int skinLevel)
        {
            foreach (var skeltonAnimation in _skeletonAnimations)
            {
                var skinName = "step" + skinLevel;
                if (skeltonAnimation.Skeleton.Data.FindSkin(skinName) == null) skinName = "default";
                skeltonAnimation.Skeleton.SetSkin(skinName);
            }
        }

        public void PauseOn()
        {
            _transform.DOPause();
            _pauseOn();
        }

        public void PauseOff()
        {
            _transform.DOPlay();
            _pauseOff();
        }
    }
}