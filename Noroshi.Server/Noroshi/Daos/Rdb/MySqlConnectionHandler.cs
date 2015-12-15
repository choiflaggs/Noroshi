using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Noroshi.Server.Daos.Rdb
{
    public class MySqlConnectionHandler : IDisposable
    {
        public enum ConnectionType
        {
            Master,
            Slave,
        }
        public enum Database
        {
            Noroshi,
            NoroshiShard1,
            NoroshiShard2,
        }

        readonly Dictionary<string, MySqlConnection> _connections = new Dictionary<string, MySqlConnection>();

        public MySqlConnection GetShardConnection(uint shardId, ConnectionType connectionType)
        {
            var database = (Database)Enum.Parse(typeof(Database), "NoroshiShard" + shardId);
            return GetConnection(database, connectionType);
        }
        public MySqlConnection GetConnection(Database database, ConnectionType connectionType)
        {
            var key = _makeConnectionKey(database, connectionType);
            return _connections.ContainsKey(key) ? _connections[key] : _connect(database, connectionType);
        }

        MySqlConnection _connect(Database database, ConnectionType connectionType)
        {
            var key = _makeConnectionKey(database, connectionType);
            var connectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            var localConnectionString = ConfigurationManager.AppSettings["ConnectionString" + key];
            if (!string.IsNullOrEmpty(localConnectionString))
            {
                connectionString = localConnectionString;
            }
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            _connections.Add(key, connection);
            return connection;
        }
        string _makeConnectionKey(Database database, ConnectionType connectionType)
        {
            return database.ToString() + connectionType.ToString();
        }

        public void Clear()
        {
            Dispose();
            _connections.Clear();
        }
        public void Dispose()
        {
            foreach (var connection in _connections.Values)
            {
                connection.Dispose();
            }
        }

        public T Transaction<T>(IEnumerable<MySqlConnection> connections, Func<TransactionContainer, T> func)
        {
            using (var transaction = new TransactionContainer(connections))
            {
                try
                {
                    var result = func.Invoke(transaction);
                    _rollback(transaction);
                    return result;
                }
                catch (Exception)
                {
                    _rollback(transaction);
                    throw;
                }
            }
        }
        void _rollback(TransactionContainer transaction)
        {
            transaction.Rollback();
        }

        public IEnumerable<dynamic> GetSchema(Database database, string schemaCollectionName)
        {
            var dataTable = GetConnection(database, ConnectionType.Slave).GetSchema(schemaCollectionName);
            return dataTable.AsEnumerable().Select(x =>
            {
                IDictionary<string, object> dict = new ExpandoObject();
                foreach (DataColumn column in x.Table.Columns)
                {
                    var value = x[column];
                    if (value is DBNull) value = null;
                    dict.Add(column.ColumnName, value);
                }
                return (dynamic)dict;
            });
        }
    }
}
