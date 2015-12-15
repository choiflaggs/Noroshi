using UnityEngine;
using DG.Tweening;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ActionRelationView : MonoBehaviour, IActionRelationView
    {
        [SerializeField] Transform[] _children;
        MeshRenderer[] _renderers;
        new void Awake()
        {
            base.Awake();
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }
        public void Appear(IActionExecutorView iexecutorView, IActionTargetView targetView, float duration)
        {
            // 固有実装過ぎるので他用途が出てきたら切り出す。
            var executorView = (CharacterView)iexecutorView;
            _gameObject.SetActive(true);
            _transform.position = ((ActionTargetView)targetView).GetPosition();
            _transform.DOMove(executorView.GetArmPosition(), duration).SetEase(Ease.InExpo);
            var randomGenerator = GlobalContainer.RandomGenerator;
            for (var i = 0; i < _children.Length; i++)
            {
                var child = _children[i];
                var delay = randomGenerator.GenerateFloat(duration / 5);
                child.DOLocalMoveY(4f / (_children.Length - 1) * i - 2, (duration / 2 - delay)).SetEase(Ease.OutExpo).SetDelay(delay);
                child.DOLocalMoveY(0, duration / 2).SetDelay(duration / 2).SetEase(Ease.InQuart);
            }
            foreach (var renderer in _renderers)
            {
                renderer.sortingOrder = executorView.GetOrderInLayer();
            }
        }
        public void PauseOn()
        {
            transform.DOPause();
            foreach (var child in _children)
            {
                child.DOPause();
            }
        }
        public void PauseOff()
        {
            transform.DOPlay();
            foreach (var child in _children)
            {
                child.DOPlay();
            }
        }
        public void Disappear()
        {
            _gameObject.SetActive(false);
        }
    }
}
