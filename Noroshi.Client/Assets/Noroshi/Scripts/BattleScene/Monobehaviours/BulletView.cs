using UnityEngine;
using UniRx;
using DG.Tweening;
using Noroshi.Grid;
using Noroshi.BattleScene.Actions;
using System.Collections.Generic;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BulletView : SpineView, Actions.IBulletView
    {
        [SerializeField] Transform[] _unreversibles;
        [Header("---attack")]
        [SerializeField] BulletMoveParam _bullet1MoveParam;
        [Header("---a_skill1")]
        [SerializeField] BulletMoveParam _bullet2MoveParam;
        [Header("---p_skill1")]
        [SerializeField] BulletMoveParam _bullet3MoveParam;
        [Header("---p_skill2")]
        [SerializeField] BulletMoveParam _bullet4MoveParam;
        [Header("---p_skill3")]
        [SerializeField] BulletMoveParam _bullet5MoveParam;

        float _estimateDuration;
        Subject<IActionTargetView> _onHitSubject;
        Subject<IBulletView> _onStockSubject;
        Dictionary<string, BulletMoveParam> _bulletMoveParamDictionary = new Dictionary<string, BulletMoveParam>();

        new void Awake()
        {
            base.Awake();
            // 設定漏れを防ぐために必要な設定はここでやってしまう。
            var rigidbody2D = GetComponent<Rigidbody2D>();
            var collider2D  = GetComponent<Collider2D>();
            rigidbody2D.gravityScale = 0;
            collider2D.isTrigger = true;
        }

        new void Start()
        {
            base.Start();
            
            _bulletMoveParamDictionary.Add(Constant.ACTION_RANK_0_ANIMATION_NAME, _bullet1MoveParam);
            _bulletMoveParamDictionary.Add(Constant.ACTION_RANK_1_ANIMATION_NAME, _bullet2MoveParam);
            _bulletMoveParamDictionary.Add(Constant.ACTION_RANK_2_ANIMATION_NAME, _bullet3MoveParam);
            _bulletMoveParamDictionary.Add(Constant.ACTION_RANK_3_ANIMATION_NAME, _bullet4MoveParam);
            _bulletMoveParamDictionary.Add(Constant.ACTION_RANK_4_ANIMATION_NAME, _bullet5MoveParam);
        }

        public IObservable<IBulletView> GetOnStock()
        {
            _onStockSubject = new Subject<IBulletView>();
            return _onStockSubject.AsObservable();
        }
        public void Stock()
        {
            SetActive(false);
            _onStockSubject.OnNext(this);
            _onStockSubject.OnCompleted();
            _onHitSubject.OnCompleted();
        }
        public IObservable<IActionTargetView> Launch(IActionExecutorView iExecutorView, IActionTargetView iTargetView, string animationName)
        {
            var bulletMoveParam = _bulletMoveParamDictionary[animationName];
            var targetView = (ActionTargetView)iTargetView;
            _setPosition(iExecutorView);
            _play(animationName);
            SetActive(true);
            // 一応、画面サイズに対応したワールド長さよりも2倍飛ばしておく。
            var distance = WaveField.HORIZONTAL_LENGTH * 2;
            var duration = distance / bulletMoveParam.Velocity;
            var vector = (targetView.GetPosition() - GetPosition()).normalized;
            var endValue = GetPosition() + vector * distance;
            _transform.DOMove(endValue, duration).SetEase(bulletMoveParam.Easing).OnComplete(() =>
            {
                Stock();
            });
            _setEulerAngle(endValue, bulletMoveParam.UseShotAngle);
            _estimateDuration = (targetView.GetPosition() - GetPosition()).magnitude / bulletMoveParam.Velocity;
            _onHitSubject = new Subject<IActionTargetView>();
            return _onHitSubject.AsObservable();
        }
        public float GetEstimateDuration()
        {
            return _estimateDuration;
        }

        void _setPosition(IActionExecutorView iExecutorView)
        {
            var view = (SpineView)iExecutorView;
            _transform.position = ((CharacterView)iExecutorView).GetArmPosition();
            SetOrderInLayer(view.GetOrderInLayer());
            _transform.rotation = view.GetTransform().rotation;
            foreach (var unreversible in _unreversibles)
            {
                unreversible.rotation = Quaternion.identity;
            }
        }

        void _setEulerAngle(Vector3 endPosition, bool useShotAngle)
        {
            if (useShotAngle)
            {
                var cur = _transform.position;
                var direction = endPosition - cur;
                var angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
                var eulerAngle = Quaternion.AngleAxis(360.0f - angle, Vector3.forward).eulerAngles;
                eulerAngle.y = 180;
                _transform.eulerAngles = eulerAngle;
            }
        }

        public void SetSkin(int skinLevel)
        {
            var skinName = "step" + skinLevel;
            if (_rootSkeletonAnimation.Skeleton.Data.FindSkin(skinName) == null) skinName = "default";
            _setRootSkin(skinName);
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

        void OnTriggerStay2D(Collider2D collider)
        {
            var targetView = collider.GetComponent<ActionTargetView>();
            if (targetView != null)
            {
                _onHitSubject.OnNext(targetView);
            }
        }
        void OnDestroy()
        {
            if (_onHitSubject != null)
            {
                _onHitSubject.OnCompleted();
                _onStockSubject.OnCompleted();
            }
        }

        [System.Serializable]
        class BulletMoveParam
        {
            [SerializeField] bool _useShotAngle = true;
            [SerializeField] float _velocity = 5.0f;
            [SerializeField] Ease _easing = Ease.Linear;

            public bool UseShotAngle { get { return _useShotAngle; } }
            public float Velocity { get { return _velocity; } }
            public Ease Easing { get { return _easing; } }
        }
    }
}
