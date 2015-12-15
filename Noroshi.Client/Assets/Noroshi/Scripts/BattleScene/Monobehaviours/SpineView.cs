using System;
using UniLinq;
using UniRx;
using UnityEngine;
using DG.Tweening;
using Spine;
using Noroshi.Grid;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class SpineView : View
    {
        protected const float SCALE = 0.22f;
        const float FADE_OUT_DURATION = 3f;
        const int TRACK_INDEX = 0;

        [SerializeField] bool _debugLog = false;
        [SerializeField] Transform[] _unreversibleTransforms;

        bool _completeInitialize = false;
        bool _activeAtStart = false;
        bool? _loop = null;
        bool _isStoppingWithPlay = false;
        Subject<bool> _onCompleteInitializeSubject = new Subject<bool>();
        Subject<bool> _onCompleteAnimationSubject = new Subject<bool>();
        protected SkeletonAnimation _rootSkeletonAnimation;
        protected SkeletonAnimation[] _skeletonAnimations;
        MeshRenderer _rootRenderer;
        MeshRenderer[] _renderers;

        protected new void Awake()
        {
            base.Awake();
            _transform.localScale = new Vector3(SCALE, SCALE, SCALE);

            _rootSkeletonAnimation = GetComponent<SkeletonAnimation>();
            _rootRenderer = GetComponent<MeshRenderer>();
        }
        protected void Start()
        {
            _skeletonAnimations = GetComponentsInChildren<SkeletonAnimation>();
            _renderers = GetComponentsInChildren<MeshRenderer>();

            // 一旦、同 GameObject が Spine アニメーションじゃなくてもエラーがでないようにしておく。
            if (_rootSkeletonAnimation == null)
            {
                _rootSkeletonAnimation = _skeletonAnimations[0];
            }
            if (_rootRenderer == null)
            {
                _rootRenderer = _renderers[0];
            }

            _rootSkeletonAnimation.state.Complete += _onAnimationComplete;
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                skeletonAnimation.skeletonDataAsset.defaultMix = 0;
                skeletonAnimation.state.ClearTracks();
            }

            _completeInitialize = true;
            SetActive(_activeAtStart);

            _onCompleteInitializeSubject.OnNext(_gameObject.activeSelf);
            _onCompleteInitializeSubject.OnCompleted();
        }

        public IObservable<bool> GetOnCompleteInitializeObservable()
        {
            return _completeInitialize ? Observable.Return(_gameObject.activeSelf) : _onCompleteInitializeSubject.AsObservable();
        }

        public override bool SetActive(bool active)
        {
            var res = false;
            if (_completeInitialize)
            {
                res = _gameObject.activeSelf != active;
                _gameObject.SetActive(active);
            }
            else
            {
                res = _activeAtStart != active;
                // Awake() 直後に呼ばれた場合、非常に重い SkeletonAnimation コンポーネントの Awake() が呼ばれる保証がない。
                // 初期化タイミングを一致させるために Start() がコールされるタイミングまで SetActive 処理を遅延させる。
                _activeAtStart = active;
            }
            return res;
        }

        public virtual void SetParent(Transform parent)
        {
            _transform.localScale = Vector3.one;
            _transform.SetParent(parent, false);
        }
        public void SetLocalPosition(Vector2 position)
        {
            _transform.localPosition = position;
        }

        /// 方向をセットする。アニメーションは左向きで作られている前提。
        public void SetHorizontalDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    _transform.eulerAngles = new Vector3(0, 180, 0);
                    _resetUnreversibleTransfromRotation();
                    break;
                case Direction.Left:
                    _transform.eulerAngles = Vector3.zero;
                    _resetUnreversibleTransfromRotation();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected void _resetUnreversibleTransfromRotation()
        {
            for (var i = 0; i < _unreversibleTransforms.Length; i++)
            {
                _unreversibleTransforms[i].rotation = Quaternion.identity;
            }
        }

        protected void _recoverFromFadeOut()
        {
            for (var i = 0; i < _skeletonAnimations.Length; i++)
            {
                _skeletonAnimations[i].skeleton.A = 1;
            }
        }
        protected IObservable<bool> _fadeOut()
        {
            var onComplete = new Subject<bool>();
            DOTween.To(() => _skeletonAnimations[0].skeleton.A, (a) =>
            {
                for (var i = 0; i < _skeletonAnimations.Length; i++)
                {
                    _skeletonAnimations[i].skeleton.A = a;
                }
            }, 0, FADE_OUT_DURATION)
            .OnComplete(() =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });
            return onComplete.AsObservable();
        }

        public int GetOrderInLayer()
        {
            return _rootRenderer.sortingOrder;
        }
        public virtual void SetOrderInLayer(int order)
        {
            var diff = order - GetOrderInLayer();
            for (var i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sortingOrder += diff;
            }
        }
        public virtual void SetSortingLayerName(string sortingLayerName)
        {
            for (var i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sortingLayerName = sortingLayerName;
            }
        }

        protected void _setRootSkin(string skinName)
        {
            _rootSkeletonAnimation.skeleton.SetSkin(skinName);
        }

        bool _isPlaying()
        {
            return _loop.HasValue;
        }
        protected IObservable<bool> _play(string animationName, bool loop = true)
        {
            if (_isPlaying()) Stop();
            if (_debugLog) GlobalContainer.Logger.Debug("Play : " + animationName);
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                skeletonAnimation.state.SetAnimation(TRACK_INDEX, animationName, loop);
            }
            _loop = loop;
            _onCompleteAnimationSubject = new Subject<bool>();
            return _onCompleteAnimationSubject.AsObservable();
        }

        void _onAnimationComplete(Spine.AnimationState state, int trackIndex, int loopCount)
        {
            if (_isPlaying() && !_loop.Value)
            {
                if (_isStoppingWithPlay)
                {
                    _stop(false);
                }
                else
                {
                    _stop(true);
                }
            }
        }
        public void Stop()
        {
            _stop(false);
        }
        void _stop(bool success)
        {
            if (_debugLog) GlobalContainer.Logger.Debug("Stop : " + _rootSkeletonAnimation.state.GetCurrent(TRACK_INDEX).animation.Name + ", Success : " + success);
            _loop = null;
            _isStoppingWithPlay = false;
            var onCompleteAnimationSubject = _onCompleteAnimationSubject;
            onCompleteAnimationSubject.OnNext(success);
            onCompleteAnimationSubject.OnCompleted();
        }

        protected void _stopWithPlay(string animationName)
        {
            if (_debugLog) GlobalContainer.Logger.Debug("Stop With Play : " + animationName);
            _isStoppingWithPlay = true;
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                skeletonAnimation.state.SetAnimation(TRACK_INDEX, animationName, false);
            }
            _loop = false;
        }

        protected void _pauseOn()
        {
            _setTimeScale(0);
        }
        protected void _pauseOff()
        {
            _setTimeScale(1);
        }
        protected void _setTimeScale(float timeScale)
        {
            foreach (var skeletonAnimation in _skeletonAnimations)
            {
                skeletonAnimation.state.TimeScale = timeScale;
            }
        }
        protected Spine.Animation _getAnimation(string animationName)
        {
            return _rootSkeletonAnimation.state.Data.skeletonData.FindAnimation(animationName);
        }
        protected EventTimeline _getAnimationEventTimeline(Spine.Animation animation)
        {
            return (EventTimeline)animation.Timelines.Where(t => t.GetType() == typeof(EventTimeline)).FirstOrDefault();
        }
    }
}
