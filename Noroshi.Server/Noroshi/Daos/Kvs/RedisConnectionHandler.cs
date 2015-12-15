using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using StackExchange.Redis;

namespace Noroshi.Server.Daos.Kvs
{
    public class RedisConnectionHandler
    {
        public enum ConnectionType
        {
            Master=0,
            Slave
        }

        static readonly IConnectionMultiplexer _redis = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_takeConnectionKeys())).Value;
        readonly Dictionary<string, IDatabase> _connections = new Dictionary<string, IDatabase>();

        public IDatabase GetConnection(ConnectionType connectionType)
        {
            var key = _makeConnectionKey(connectionType);
            return _connections.ContainsKey(key) ? _connections[key] : _connect(connectionType);
        }

        IDatabase _connect(ConnectionType connectionType)
        {
            var key = _makeConnectionKey(connectionType);
            var connection = _redis.GetDatabase((int)connectionType);
            _connections.Add(key, connection);
            return connection;
        }

        static string _makeConnectionKey(ConnectionType connectionType)
        {
            return "NoroshiKvs" + connectionType;
        }

        static string _takeConnectionKeys()
        {
            var strings = Enum.GetValues(typeof (ConnectionType)).Cast<ConnectionType>().ToList().Select(type =>
            {
                var key = _makeConnectionKey(type);
                var connectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                var localConnectionString = ConfigurationManager.AppSettings["ConnectionString" + key];
                return string.IsNullOrEmpty(localConnectionString) ? connectionString : localConnectionString;
            });
            return string.Join(",", strings);
        }
    }
}
