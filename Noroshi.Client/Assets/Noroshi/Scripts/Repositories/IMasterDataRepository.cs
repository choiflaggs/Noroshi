using UniRx;

namespace Noroshi.Repositories
{
    public interface IMasterDataRepository<T>
    {
        IObservable<T[]> LoadAll();
        IObservable<T> Get(uint id);
        IObservable<T[]> GetMulti(uint[] ids);
    }
}