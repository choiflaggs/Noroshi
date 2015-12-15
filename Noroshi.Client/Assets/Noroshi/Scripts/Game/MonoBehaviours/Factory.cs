using UnityEngine;
using UniRx;

namespace Noroshi.Game.MonoBehaviours
{
    public class Factory : MonoBehaviour
    {
        protected IObservable<T> _loadFromResource<T>(string path)
            where T : Component
        {
            var go = Instantiate(Resources.Load(path)) as GameObject;
            return Observable.Return<T>(go.GetComponent<T>());
        }
    }
}