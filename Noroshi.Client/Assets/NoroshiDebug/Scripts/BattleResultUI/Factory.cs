using UnityEngine;
using UniRx;
using Noroshi.BattleScene.UI;
using Noroshi.BattleScene.MonoBehaviours;

namespace NoroshiDebug.BattleResultUI
{
    public class Factory : Noroshi.Game.MonoBehaviours.Factory
    {
        [SerializeField] UIController _UIController;
        public IObservable<UIController> LoadUIController()
        {
            return Observable.Return<UIController>(_UIController);
        }
        public IObservable<IResultUIView> LoadResultUIView(string name)
        {
            var path = string.Format("Battle/UI/Result/{0}", name);
            return _loadFromResource<ResultUIView>(path).Cast<ResultUIView, IResultUIView>();
        }
    }
}