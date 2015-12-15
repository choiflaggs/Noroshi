using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniLinq;
using DG.Tweening;
using Noroshi.Grid;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class CharacterView : SpineView, ICharacterView
    {
        const string ACTION_EVENT_NAME = "firing";
        const string ACTION_EVENT_NAME_FOR_MOVE = "move";
        const string EXIT_TIME_STOP_EVENT_NAME = "starting";
        const string DAMAGE_AREA_NAME = "damage_area";

        [SerializeField] Transform _arm;
        [SerializeField] float _effectTopLocalY = 0;
        [SerializeField] float _effectFrontLocalX = 0;
        [SerializeField] float _effectScale = 1f;
        [SerializeField] Vector2 _centerPositionForUIWithStory;
        [SerializeField] Vector2 _centerPositionForUIWithResult;
        [SerializeField] float _scaleForUI = 0.75f;
        [SerializeField] bool _debugSkinChangeOff = false;
        [Header("---子がStep付きのSkeletonAnimationを持つ場合セット")]
        [SerializeField] SkeletonAnimation[] _skinSettableSkeletonAnimations;

        Dictionary<string, bool> _actionAnimationNames = new Dictionary<string, bool>();
        BoundingBoxFollower _boundingBoxFollower;
        Transform _body;
        ActionTargetView  _actionTargetView;
        Subject<bool> _onActionAnimationExit = new Subject<bool>();
        Subject<bool> _onActionInvocation    = new Subject<bool>();
        Subject<bool> _onExitTimeStop        = new Subject<bool>();
        Subject<bool> _onWinAnimationExit    = new Subject<bool>();
        Subject<bool> _onExitDeadAnimation;
        List<Tweener> _moveTweeners = new List<Tweener>();
        Subject<bool> _onCompleteMoveSubject = new Subject<bool>();
        Queue<Vector2> _positions;
        IDisposable _moveDisposable;
        /// 死亡前逃亡アニメーションフラグ。
        bool _escapeBeforeDead;

        new void Awake()
        {
            base.Awake();
            // コスト削減のためのマップ作成
            var actionAnimationNames = (new string[]{
                Constant.ACTION_RANK_0_ANIMATION_NAME,
                Constant.ACTION_RANK_1_ANIMATION_NAME,
                Constant.ACTION_RANK_2_ANIMATION_NAME,
                Constant.ACTION_RANK_3_ANIMATION_NAME,
                Constant.ACTION_RANK_4_ANIMATION_NAME,
                Constant.ACTION_RANK_5_ANIMATION_NAME,
                Constant.APPEAR_ANIMATION_NAME,
            });
            foreach (var animationName in actionAnimationNames)
            {
                _actionAnimationNames.Add(animationName, true);
            }
        }
        new void Start()
        {
            // 各コンポーネントをキャッシュ
            _boundingBoxFollower = GetComponentInChildren<BoundingBoxFollower>();
            _body     = GetComponentInChildren<BoundingBoxFollower>().transform;
            _actionTargetView  = GetComponentInChildren<ActionTargetView>();

            base.Start();
            // コールバックをセット
            _rootSkeletonAnimation.state.Event += _onAnimationEvent;
            // デフォルトは Idle
            PlayIdle();
        }

        public override bool SetActive(bool active)
        {
            if (_boundingBoxFollower != null)
            {
                GameObject.DestroyImmediate(_boundingBoxFollower);
                _boundingBoxFollower = _body.gameObject.AddComponent<BoundingBoxFollower>();
                _boundingBoxFollower.slotName = DAMAGE_AREA_NAME;
            }
            return base.SetActive(active);
        }

        /// アクションアニメーション終了タイミングでプッシュされる Observable を取得。
        public IObservable<bool> GetOnExitActionAnimationObservable()
        {
            return _onActionAnimationExit.AsObservable();
        }
        /// アクションアニメーションで効果を発動させたいタイミングでプッシュされる Observable を取得。
        public IObservable<bool> GetOnInvokeActionObservable()
        {
            return _onActionInvocation.AsObservable();
        }
        public IObservable<bool> GetOnExitTimeStopObservable()
        {
            return _onExitTimeStop.AsObservable();
        }
        public IObservable<bool> GetOnExitWinAnimationObservable()
        {
            return _onWinAnimationExit.AsObservable();
        }

        public IObservable<BulletHitEvent> GetOnHitObservable()
        {
            return _actionTargetView.GetOnHitObservable();
        }

        public IEnumerable<ActionAnimation> GetActionAnimations()
        {
            return _actionAnimationNames.Keys.Select(name => _getActionAnimation(name)).Where(a => a != null);
        }
        ActionAnimation _getActionAnimation(string animationName)
        {
            var animation = _getAnimation(animationName);
            if (animation == null) return null;
            var eventTimeline = _getAnimationEventTimeline(animation);
            if (eventTimeline == null) return null;
            var actionTriggers = new List<float>();
            for (var i = 0; i < eventTimeline.FrameCount; i++)
            {
                if (eventTimeline.Events[i].ToString() == ACTION_EVENT_NAME || eventTimeline.Events[i].ToString() == ACTION_EVENT_NAME_FOR_MOVE)
                {
                    actionTriggers.Add(eventTimeline.Frames[i]);
                }
            }
            return new ActionAnimation(animationName, actionTriggers.ToArray(), animation.Duration);
        }

        /// ポーズ
        public void PauseOn()
        {
            _pauseOn();
            foreach (var moveTweener in _moveTweeners)
            {
                moveTweener.Pause();
            }
        }
        /// ポーズ解除
        public void PauseOff()
        {
            foreach (var moveTweener in _moveTweeners)
            {
                moveTweener.Play();
            }
            _pauseOff();
        }

        public void SetSpeed(float speed)
        {
            _setTimeScale(speed);
        }

        /// 死亡前逃走アニメーションフラグを立てる。
        public void SetEscapeBeforeDeadOn()
        {
            _escapeBeforeDead = true;
        }

        public IObservable<bool> Move(Vector2 position, float duration)
        {
            return Move(new Vector2[]{position}, duration);
        }
        public IObservable<bool> Move(IEnumerable<Vector2> positions, float duration)
        {
            StopMove();
            _positions = new Queue<Vector2>(positions);
            var position = positions.Last();

            var positionNum = _positions.Count;
            var onCompleteMoveSubject = new Subject<bool>();
            _moveTweeners.Add(_transform.DOMove(position, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                for (var i = 0; i < positionNum; i++)
                {
                    onCompleteMoveSubject.OnNext(true);
                }
                onCompleteMoveSubject.OnCompleted();
            }));

            _moveDisposable = Observable.EveryUpdate().Scan((Vector2)_transform.position, (prev, _) =>
            {
                var cur = (Vector2)_transform.position;
                while (_positions.Count() > 0)
                {
                    var candidate = _positions.FirstOrDefault();
                    if ((prev.x <= candidate.x && candidate.x < cur.x) || (cur.x < candidate.x && candidate.x <= prev.x))
                    {
                        _positions.Dequeue();
                        if (_positions.Count() > 0)
                        {
                            positionNum--;
                            onCompleteMoveSubject.OnNext(true);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return cur;
            })
            .Skip(1)
            .Subscribe();
            _onCompleteMoveSubject = onCompleteMoveSubject;

            return onCompleteMoveSubject;
        }
        public void StopMove()
        {
            foreach (var moveTweener in _moveTweeners)
            {
                moveTweener.Kill();
            }
            _moveTweeners.Clear();
            _onCompleteMoveSubject.OnCompleted();
            if (_moveDisposable != null) _moveDisposable.Dispose();
        }

        public void PlayIdle()
        {
            _playAnimation(Constant.IDLE_ANIMATION_NAME);
        }
        public void PlayWalk()
        {
            _playAnimation(Constant.WALK_ANIMATION_NAME);
        }
        public IObservable<bool> WalkTo(Vector2 position, float duration)
        {
            _playAnimation(Constant.WALK_ANIMATION_NAME);
            return Move(position, duration).Do(_ => PlayIdle());
        }
        public IObservable<bool> RunTo(Vector2 position, float duration)
        {
            _playAnimation(Constant.RUN_ANIMATION_NAME);
            return Move(position, duration).Do(_ => PlayIdle());
        }
        public void PlayAction(string actionAnimationName)
        {
            _play(actionAnimationName, false).Subscribe(success => _onActionAnimationExit.OnNext(success));
        }
        public void PlayDamage()
        {
            _playAnimationOnce(Constant.DAMAGE_ANIMATION_NAME);
        }
        public void PlayKnockback(Vector2 position, float duration)
        {
            _playAnimationOnce(Constant.DAMAGE_ANIMATION_NAME);
            StopMove();
            var firstDuration = duration / 2;
            var diffY = 1f;
            _moveTweeners.Add(_transform.DOLocalMoveX(position.x, duration));
            _moveTweeners.Add(_transform.DOLocalMoveY(position.y + diffY, firstDuration).SetEase(Ease.OutExpo));
            _moveTweeners.Add(_transform.DOLocalMoveY(position.y, duration - firstDuration).SetDelay(firstDuration).SetEase(Ease.OutBounce));
        }
        public void PlayStun()
        {
            _playAnimationOnce(Constant.STUN_ANIMATION_NAME);
        }
        public void PlayVanish()
        {
            SetActive(false);
            SceneContainer.GetTimeHandler().Timer(Constant.VANISH_TIME)
            .Do(t => SetActive(true))
            .Subscribe();
        }
        /// 死亡状態に遷移した際のアニメーションを再生する。
        public IObservable<bool> PlayAnimationAtEnterDead()
        {
            _onExitDeadAnimation = new Subject<bool>();
            /// 死亡前逃走アニメーションフラグが立っている場合、
            /// ダメージアニメーションを一回再生後、待機アニメーションをループ再生。
            /// アニメーション完了時の OnNext はここでせず、死亡前逃走アニメーション再生に任せる。
            if (_escapeBeforeDead)
            {
                _play(Constant.DAMAGE_ANIMATION_NAME, false)
                .SelectMany(_play(Constant.IDLE_ANIMATION_NAME, true))
                .Subscribe().AddTo(_gameObject);
            }
            else
            {
                /// 死亡アニメーション再生後にフェードアウト。完了後にアニメーション完了時の OnNext をする。
                _play(Constant.DEAD_ANIMATION_NAME, false).SelectMany(_ => _fadeOut())
                .Subscribe(_ =>
                {
                   _onExitDeadAnimation.OnNext(true);
                   _onExitDeadAnimation.OnCompleted(); 
                })
                .AddTo(_gameObject);
            }
            return _onExitDeadAnimation.AsObservable();
        }
        /// 死亡前逃走アニメーションを再生する。
        public void PlayEscapeBeforeDead()
        {
            SetHorizontalDirection(Direction.Right);
            _playAnimation(Constant.RUN_ANIMATION_NAME);
            Move(GetPosition() + Vector2.right * WaveField.HORIZONTAL_LENGTH * 2, Constant.BOSS_ESCAPE_TIME)
            .Subscribe(_ =>
            {
               _onExitDeadAnimation.OnNext(true);
               _onExitDeadAnimation.OnCompleted(); 
            })
            .AddTo(_gameObject);
        }
        public IObservable<bool> PlayEscape()
        {
            return _play(Constant.ESCAPE_ANIMATION_NAME, false);
        }
        public void PlayWin()
        {
            _play(Constant.WIN_ANIMATION_NAME, false).Subscribe(success => _onWinAnimationExit.OnNext(success));
        }

        public void Resurrect()
        {
            _recoverFromFadeOut();
        }

        void _playAnimation(string name)
        {
            _play(name, true).Subscribe();
        }
        void _playAnimationOnce(string name)
        {
            _play(name, false).Subscribe();
        }

        public void SetSkin(byte skinLevel)
        {
            if (_debugSkinChangeOff) return;
            _setRootSkin("step" + skinLevel);
            // 呼ばれる段階ではまだ_skeletonAnimationsがnullなので別で設定した方を利用する
            foreach (var skeltonAnimation in _skinSettableSkeletonAnimations)
            {
                var skinName = "step" + skinLevel;
                if (skeltonAnimation.Skeleton.Data.FindSkin(skinName) == null) skinName = "default";
                skeltonAnimation.Skeleton.SetSkin(skinName);
            }
        }
        public Vector2 GetLocalPosition(PositionInCharacterView position)
        {
            var localPosition = default(Vector2);
            switch (position)
            {
                case PositionInCharacterView.Top:
                    localPosition = GetBodyLocalPosition() + Vector2.up * (_effectTopLocalY > 0 ? _effectTopLocalY : GetBodyLocalPosition().y);
                    break;
                case PositionInCharacterView.Center:
                    localPosition = GetBodyLocalPosition();
                    break;
                case PositionInCharacterView.Bottom:
                    localPosition = Vector2.zero;
                    break;
                case PositionInCharacterView.Front:
                    localPosition = GetBodyLocalPosition() + Vector2.left * _effectFrontLocalX;
                    break;
                default:
                    break;
            }
            return localPosition;
        }
        public float GetEffectScale()
        {
            return _effectScale;
        }

        public Vector2 GetBodyLocalPosition()
        {
            return _body.localPosition;
        }
        public Vector2 GetTopPosition()
        {
            var dir = 0.0f;
            switch((int)_transform.eulerAngles.y)
            {
            case 0:
                dir = 1.0f;
                break;
            case 180:
                dir = -1.0f;
                break;
            default:
                throw new NotImplementedException();
            }
            var localPosition = GetLocalPosition(PositionInCharacterView.Top) * SCALE;
            localPosition.Set(localPosition.x * dir, localPosition.y);
            return GetPosition() + localPosition;
        }
        public Vector2 GetCenterPosition()
        {
            return _body.position;
        }
        public Vector2 GetArmPosition()
        {
            return _arm.position;
        }

        // アニメーション内に仕込まれたイベントをハンドリング
        void _onAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
        {
            if (e.ToString() == ACTION_EVENT_NAME || e.ToString() == ACTION_EVENT_NAME_FOR_MOVE)
            {
                _onActionInvocation.OnNext(true);
            }
            else if (e.ToString() == EXIT_TIME_STOP_EVENT_NAME)
            {
                _onExitTimeStop.OnNext(true);
            }
        }

        public IActionTargetView GetActionTargetView()
        {
            return _actionTargetView;
        }

        /// UI レイヤーに表示するために変換する。
        public void SetUpForUI(CenterPositionForUIType centerPositionForUIType)
        {
            _transform.localScale = Vector3.one * _scaleForUI;
            var centerPositionForUI = Vector2.zero;
            switch (centerPositionForUIType)
            {
            case CenterPositionForUIType.Story:
                centerPositionForUI = _centerPositionForUIWithStory;
                break;
            case CenterPositionForUIType.Result:
                centerPositionForUI = _centerPositionForUIWithResult;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
            _transform.localPosition = new Vector3(-centerPositionForUI.x , -centerPositionForUI.y, _transform.localPosition.z);
            SetSortingLayerName(Constant.UI_SORTING_LAYER_NAME);
            SetOrderInLayer(Constant.SPINE_UI_ORDER_IN_LAYER);
            SetSkin(1);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_moveDisposable != null) _moveDisposable.Dispose();
        }
    }
}
