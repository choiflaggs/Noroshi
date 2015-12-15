using System;
using UniRx;

namespace Noroshi.BattleScene.Cache
{
    public interface ICache<T> : IDisposable where T : IDisposable
    {
        IObservable<T> Get();
        void Stock(T content);
    }
}