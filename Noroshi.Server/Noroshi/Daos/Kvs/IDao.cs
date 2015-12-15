using System.Collections.Generic;

namespace Noroshi.Server.Daos.Kvs
{
    public interface IDao<TKey, TValue> where TValue : class
    {
        TValue Get(TKey key);
        Dictionary<TKey, TValue> GetMulti(IEnumerable<TKey> keys);
        bool Set(TKey key, TValue value);
        IEnumerable<bool> SetMulti(Dictionary<TKey, TValue> map);
        bool Remove(TKey key);
    }
}
