using System;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.BattleScene.Cache
{
    public abstract class AbstractCache<T> : ICache<T> where T : IDisposable
    {
        List<T> _contents = new List<T>();

        public IObservable<int> Preload(int num)
        {
            var loadNum = num - _contents.Count;
            if (loadNum < 0) loadNum = 0;
            IObservable<T>[] observables = new IObservable<T>[num];
            for (var i = 0; i < num; i++)
            {
                observables[i] = Get();
            }
            return _loadContents(num).Select(cs =>
            {
                _contents.AddRange(cs);
                return loadNum;                
            });
        }

        protected virtual IObservable<T[]> _loadContents(int num)
        {
            IObservable<T>[] observables = new IObservable<T>[num];
            for (var i = 0; i < num; i++)
            {
                observables[i] = _loadContent();
            }
            return Observable.WhenAll(observables);
        }

        public IObservable<T> Get()
        {
            if (_contents.Count > 0)
            {
                var content = _contents[0];
                _contents.Remove(content);
                return Observable.Return<T>(content);
            }
            else
            {
                return _loadContent();
            }
        }

        public void Stock(T content)
        {
            _contents.Add(content);
        }

        protected abstract IObservable<T> _loadContent();

        public void Dispose()
        {
            foreach (var content in _contents)
            {
                content.Dispose();
            }
            _contents.Clear();
        }
    }
}