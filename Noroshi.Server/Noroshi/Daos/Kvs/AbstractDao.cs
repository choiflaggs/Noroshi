using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MsgPack.Serialization;
using StackExchange.Redis;

namespace Noroshi.Server.Daos.Kvs
{
    public abstract class AbstractDao<TKey, TValue> : IDao<TKey, TValue> where TValue : class
    {
        private RedisConnectionHandler connection = new RedisConnectionHandler();

        public TValue Get(TKey key)
        {
            var value = _getMasterDbConnection().StringGetAsync(key.ToString()).Result;
            if (value.IsNullOrEmpty) return null;
            var stream = new MemoryStream(value);
            var serializer = SerializationContext.Default.GetSerializer<TValue>();
            return serializer.Unpack(stream);
        }

        public Dictionary<TKey, TValue> GetMulti(IEnumerable<TKey> keys)
        {
            var values = Task.WhenAll(keys.Select(key => _getMasterDbConnection().StringGetAsync(key.ToString()))).Result;
            if (values.All(value => value.IsNullOrEmpty)) return null;
            return values.Select(value =>
            {
                var stream = new MemoryStream(value);
                var serializer = SerializationContext.Default.GetSerializer<TValue>();
                return serializer.Unpack(stream);
            }).Zip(keys, (v, k) => new { v, k }).ToDictionary(a => a.k, a => a.v);
        }

        public bool Set(TKey key, TValue value)
        {
            var stream = new MemoryStream();
            var serializer = SerializationContext.Default.GetSerializer<TValue>();
            serializer.Pack(stream, value);
            return _getMasterDbConnection().StringSetAsync(key.ToString(), stream.GetBuffer()).Result;
        }

        public IEnumerable<bool> SetMulti(Dictionary<TKey, TValue> maps)
        {
            var values = new Dictionary<TKey, byte[]>();
            foreach (var map in maps)
            {
                var stream = new MemoryStream();
                var serializer = SerializationContext.Default.GetSerializer<TValue>();
                serializer.Pack(stream, map.Value);
                values.Add(map.Key, stream.GetBuffer());
            }
            return Task.WhenAll(values.Select(value => _getMasterDbConnection().StringSetAsync(value.Key.ToString(), value.Value))).Result;
        }

        public bool Remove(TKey key)
        {
            return _getMasterDbConnection().KeyDeleteAsync(key.ToString()).Result;
        }

        private IDatabase _getMasterDbConnection()
        {
            return
                connection.GetConnection(RedisConnectionHandler.ConnectionType.Master);
        }

        private IDatabase _getSleveDbConnection()
        {
            return
                connection.GetConnection(RedisConnectionHandler.ConnectionType.Slave);
        }
    }
}