using UnityEngine;
using UniRx;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class DropMoneyUIView : UIView, IDropMoneyUIView
    {
        [SerializeField] float _lifeTime = 1f;
        Subject<bool> _onDisposeSubject = new Subject<bool>();

        new void Awake()
        {
            base.Awake();
            SetActive(false);
        }

        public IObservable<bool> Drop(ICharacterView characterView)
        {
            SetActive(true);
            Invoke("Dispose", _lifeTime);
            return _onDisposeSubject.AsObservable();
        }
        public override void Dispose()
        {
            _onDisposeSubject.OnNext(true);
            _onDisposeSubject.OnCompleted();
            base.Dispose();
        }
    }
}